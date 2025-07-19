using Microsoft.Extensions.Logging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Yape.Library.Soap.Client.Infrastructure.Adapters.Out.SoapClient
{
    public static class SOAPMessageLoggingBehaviorFactory
    {
        public static SOAPMessageLoggingBehaviour GetLoggingBehavior(string adapterName, ILogger logger)
        {
            return new SOAPMessageLoggingBehaviour(adapterName, logger);
        }
    }

    public class SOAPMessageLoggingBehaviour : IClientMessageInspector, IEndpointBehavior
    {
        private readonly string AdapterName;
        private readonly ILogger Logger;

        internal SOAPMessageLoggingBehaviour(string adapterName, ILogger logger)
        {
            AdapterName = adapterName;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public object? BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(int.MaxValue);
            request = buffer.CreateMessage();

            XmlDictionaryReader xdr = buffer.CreateMessage().GetReaderAtBodyContents();
            XNode xn = XNode.ReadFrom(xdr);
            string s = xn.ToString();

            var newXmlString = Regex.Replace(s, "<password>[\\S\\s]*?</password>\\s*", "<password>HIDDEN</password>");

            Logger.LogInformation("[{AdapterName} SOAP request] {Request}", AdapterName, newXmlString);

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue);
            reply = buffer.CreateMessage();

            XmlDictionaryReader xdr = buffer.CreateMessage().GetReaderAtBodyContents();
            XNode xn = XNode.ReadFrom(xdr);
            string responseContent = xn.ToString();

            Logger.LogInformation("[{AdapterName} SOAP response] {Response}", AdapterName, responseContent);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
