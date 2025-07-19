using System.ServiceModel.Channels;

namespace Yape.Library.Soap.Client.Application.Ports.Out.Options
{
    public class SoapClientOptions
    {
        public required string Endpoint { get; set; }

        public Binding? Binding { get; set; }

        public TimeSpan TimeOutSeconds { get; set; } = TimeSpan.FromSeconds(30);

        public string DefaultNamespace { get; set; } = "http://tempuri.org/";

        public int MaxBufferSize { get; set; } = 12000000;

        public int MaxBufferPoolSize { get; set; } = 524288;

        public bool BypassProxyOnLocal { get; set; } = false;

        public bool UseDefaultWebProxy { get; set; } = true;

        public bool AllowCookies { get; set; } = false;

        public Dictionary<string, string>? CustomHeaders { get; set; }

        public PollySettings PollySettings { get; set; } = new();
    }

    public class PollySettings
    {
        public int RetryCount { get; set; } = 3;

        public int WaitAndRetrySeconds { get; set; } = 4;
    }
}
