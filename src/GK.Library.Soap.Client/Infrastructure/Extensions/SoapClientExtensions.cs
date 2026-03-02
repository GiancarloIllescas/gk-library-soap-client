using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.ServiceModel;
using GK.Library.Soap.Client.Application.Ports.Out;
using GK.Library.Soap.Client.Application.Ports.Out.Options;
using GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient;

namespace GK.Library.Soap.Client.Infrastructure.Extensions
{
    public static class SoapClientExtensions
    {
        public static IServiceCollection AddSoapClientService<TClient, TChannel>(this IServiceCollection services, Action<SoapClientOptions> configure)
            where TClient : ClientBase<TChannel>
            where TChannel : class
        {
            var options = new SoapClientOptions
            {
                Endpoint = string.Empty
            };
            configure(options);

            services.AddScoped<ISoapClientService, SoapClientService>();

            services.AddScoped(provider =>
            {
                var asyncPolicy = Policy
                    .Handle<TimeoutException>()
                    .Or<CommunicationException>()
                    .WaitAndRetryAsync(options.PollySettings.RetryCount, attempt => TimeSpan.FromSeconds(options.PollySettings.WaitAndRetrySeconds));

                var syncPolicy = Policy
                    .Handle<TimeoutException>()
                    .Or<CommunicationException>()
                    .Retry(options.PollySettings.RetryCount);

                var factory = provider.GetRequiredService<ISoapClientService>();

                return factory.CreateClient<TClient, TChannel>(options, asyncPolicy, syncPolicy);
            });

            return services;
        }
    }
}
