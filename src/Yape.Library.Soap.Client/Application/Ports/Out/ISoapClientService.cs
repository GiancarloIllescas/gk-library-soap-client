using Polly;
using System.ServiceModel;
using Yape.Library.Soap.Client.Application.Ports.Out.Options;

namespace Yape.Library.Soap.Client.Application.Ports.Out
{
    public interface ISoapClientService
    {
        TChannel CreateClient<TClient, TChannel>(SoapClientOptions options, IAsyncPolicy asyncPolicy, ISyncPolicy? syncPolicy = null)
            where TClient : ClientBase<TChannel>
            where TChannel : class;
    }
}
