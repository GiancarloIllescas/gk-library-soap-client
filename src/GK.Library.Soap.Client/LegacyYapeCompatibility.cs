// Obsolete forwarding types to preserve binary compatibility after renaming from Yape to GK
#pragma warning disable CS0612

namespace Yape.Library.Soap.Client.Application.Ports.Out
{
    [System.Obsolete("Use GK.Library.Soap.Client.Application.Ports.Out.ISoapClientService")]
    public interface ISoapClientService : GK.Library.Soap.Client.Application.Ports.Out.ISoapClientService
    {
    }
}

namespace Yape.Library.Soap.Client.Application.Ports.Out.Options
{
    [System.Obsolete("Use GK.Library.Soap.Client.Application.Ports.Out.Options.SoapClientOptions")]
    public class SoapClientOptions : GK.Library.Soap.Client.Application.Ports.Out.Options.SoapClientOptions
    {
        public SoapClientOptions() : base() { }
    }

    [System.Obsolete("Use GK.Library.Soap.Client.Application.Ports.Out.Options.PollySettings")]
    public class PollySettings : GK.Library.Soap.Client.Application.Ports.Out.Options.PollySettings
    {
        public PollySettings() : base() { }
    }
}

namespace Yape.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient
{
    [System.Obsolete("Use GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SoapClientService")]
    public class SoapClientService : GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SoapClientService
    {
        public SoapClientService(Microsoft.Extensions.Logging.ILogger<GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SoapClientService> logger)
            : base(logger) { }
    }

    [System.Obsolete("Use GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SOAPMessageLoggingBehaviorFactory")]
    public static class SOAPMessageLoggingBehaviorFactory
    {
        public static SOAPMessageLoggingBehaviour GetLoggingBehavior(string adapterName, Microsoft.Extensions.Logging.ILogger logger)
            => new SOAPMessageLoggingBehaviour(adapterName, logger);
    }

    [System.Obsolete("Use GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SOAPMessageLoggingBehaviour")]
    public class SOAPMessageLoggingBehaviour : GK.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient.SOAPMessageLoggingBehaviour
    {
        internal SOAPMessageLoggingBehaviour(string adapterName, Microsoft.Extensions.Logging.ILogger logger)
            : base(adapterName, logger) { }
    }
}

namespace Yape.Library.Soap.Client.Infrastructure.Extensions
{
    [System.Obsolete("Use GK.Library.Soap.Client.Infrastructure.Extensions.SoapClientExtensions")]
    public static class SoapClientExtensions
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddSoapClientService<TClient, TChannel>(
            this Microsoft.Extensions.DependencyInjection.IServiceCollection services,
            System.Action<GK.Library.Soap.Client.Application.Ports.Out.Options.SoapClientOptions> configure)
            where TClient : System.ServiceModel.ClientBase<TChannel>
            where TChannel : class
            => GK.Library.Soap.Client.Infrastructure.Extensions.SoapClientExtensions.AddSoapClientService<TClient, TChannel>(services, configure);
    }
}

#pragma warning restore CS0612
