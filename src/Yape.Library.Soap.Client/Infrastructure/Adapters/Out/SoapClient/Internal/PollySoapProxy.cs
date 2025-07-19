using Polly;
using System.Reflection;

namespace Yape.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.Internal
{
    internal class PollySoapProxy<TChannel> : DispatchProxy where TChannel : class
    {
        private TChannel _decorated = default!;
        private IAsyncPolicy _asyncPolicy = default!;
        private ISyncPolicy? _syncPolicy;

        public void Configure(TChannel decorated, IAsyncPolicy asyncPolicy, ISyncPolicy? syncPolicy = null)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _asyncPolicy = asyncPolicy ?? throw new ArgumentNullException(nameof(asyncPolicy));
            _syncPolicy = syncPolicy;
        }

        protected override object? Invoke(MethodInfo targetMethod, object?[]? args)
        {
            var returnType = targetMethod.ReturnType;

            if (returnType == typeof(Task))
            {
                return _asyncPolicy.ExecuteAsync(async () =>
                {
                    var result = (Task)targetMethod.Invoke(_decorated, args)!;
                    await result.ConfigureAwait(false);
                });
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return ExecuteGenericTaskAsync(returnType, targetMethod, args!);
            }

            if (_syncPolicy is not null)
            {
                return _syncPolicy.Execute(() =>
                {
                    return targetMethod.Invoke(_decorated, args);
                });
            }
            else
            {
                return targetMethod.Invoke(_decorated, args);
            }
        }

        private object ExecuteGenericTaskAsync(Type returnType, MethodInfo method, object[] args)
        {
            var genericType = returnType.GetGenericArguments()[0];

            var genericMethod = typeof(PollySoapProxy<TChannel>)
                .GetMethod(nameof(ExecuteGenericTaskAsyncInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(genericType);

            return genericMethod.Invoke(this, new object[] { method, args })!;
        }

        private async Task<T> ExecuteGenericTaskAsyncInternal<T>(MethodInfo method, object[] args)
        {
            return await _asyncPolicy.ExecuteAsync(async () =>
            {
                var result = method.Invoke(_decorated, args);

                if (result is Task<T> task)
                {
                    return await task.ConfigureAwait(false);
                }

                throw new InvalidOperationException($"Expected Task<{typeof(T).Name}> from method {method.Name}.");
            });
        }

        public static TChannel Create(TChannel decorated, IAsyncPolicy asyncPolicy, ISyncPolicy? syncPolicy = null)
        {
            var proxy = Create<TChannel, PollySoapProxy<TChannel>>();

            ((PollySoapProxy<TChannel>)(object)proxy!).Configure(decorated, asyncPolicy, syncPolicy);

            return proxy!;
        }
    }
}
