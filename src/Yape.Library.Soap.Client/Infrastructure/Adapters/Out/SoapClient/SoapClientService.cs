using Microsoft.Extensions.Logging;
using Polly;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Yape.Library.Soap.Client.Application.Ports.Out;
using Yape.Library.Soap.Client.Application.Ports.Out.Options;
using Yape.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.Internal;

namespace Yape.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient
{
    public class SoapClientService : ISoapClientService
    {
        private readonly ILogger<SoapClientService> _logger;

        public SoapClientService(ILogger<SoapClientService> logger)
        {
            _logger = logger;
            _logger.LogInformation("SoapClientFactory initialized");
        }

        public TChannel CreateClient<TClient, TChannel>(SoapClientOptions options, IAsyncPolicy asyncPolicy, ISyncPolicy? syncPolicy = null)
            where TClient : ClientBase<TChannel>
            where TChannel : class
        {
            try
            {
                _logger.LogDebug("Creating SOAP client for {ServiceUrl}", options.Endpoint);

                if (options == null) throw new ArgumentNullException(nameof(options));
                if (string.IsNullOrWhiteSpace(options.Endpoint)) throw new ArgumentException("Missing endpoint.");

                var timeOutValue = options.TimeOutSeconds;
                var binding = options.Binding ?? GetBinding(options);
                var endpoint = new EndpointAddress(options.Endpoint);

                var client = Activator.CreateInstance(typeof(TClient), binding, endpoint) as TClient;

                if (client == null)
                {
                    throw new InvalidOperationException($"Failed to create instance of {typeof(TClient).Name}");
                }

                // Configurar headers personalizados
                //Add headers if have
                if (options.CustomHeaders != null && options.CustomHeaders.Count != 0)
                {
                    using (var operationContextScope = new OperationContextScope(client.InnerChannel))
                    {
                        var requestMessage = OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name]
                        as HttpRequestMessageProperty ?? new HttpRequestMessageProperty();

                        foreach (var key in options.CustomHeaders)
                        {
                            requestMessage.Headers.Add(key.Key, key.Value);
                        }

                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                    }
                }

                var loggingBehaviour = SOAPMessageLoggingBehaviorFactory.GetLoggingBehavior(nameof(TClient), _logger);

                client.Endpoint.EndpointBehaviors.Add(loggingBehaviour);

                _logger.LogInformation("SOAP client for {ServiceUrl} created successfully", options.Endpoint);

                var rawClient = (TChannel)(object)client;
                var proxiedClient = PollySoapProxy<TChannel>.Create(rawClient, asyncPolicy, syncPolicy);

                return proxiedClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating SOAP client for {ServiceUrl}", options.Endpoint);
                throw;
            }
        }

        private Binding GetBinding(SoapClientOptions options)
        {
            return new BasicHttpBinding
            {
                Name = "SoapBinding",
                Security = new BasicHttpSecurity
                {
                    Mode = options.Endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                        ? BasicHttpSecurityMode.Transport
                        : BasicHttpSecurityMode.None,
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.None,
                        ProxyCredentialType = HttpProxyCredentialType.None
                    }
                },
                CloseTimeout = options.TimeOutSeconds,
                OpenTimeout = options.TimeOutSeconds,
                ReceiveTimeout = options.TimeOutSeconds,
                SendTimeout = options.TimeOutSeconds,
                BypassProxyOnLocal = options.BypassProxyOnLocal,
                MaxBufferSize = options.MaxBufferSize,
                MaxBufferPoolSize = options.MaxBufferPoolSize,
                MaxReceivedMessageSize = options.MaxBufferSize,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = options.UseDefaultWebProxy,
                AllowCookies = options.AllowCookies,
                ReaderQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxDepth = 32,
                    MaxStringContentLength = 8192000,
                    MaxArrayLength = 16384,
                    MaxBytesPerRead = 4096,
                    MaxNameTableCharCount = 16384
                },
                Namespace = options.DefaultNamespace
            };
        }
    }
}
