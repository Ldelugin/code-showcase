using System.ServiceModel;

namespace Utility.Libraries.ServiceReferences;

public static class Constants
{
    private const int MaxMessageSize = 2000000000;
    private static readonly TimeSpan DefaultTimeout = new(hours: 0, minutes: 10, seconds: 0);

    public const string DefaultQueue = "default";
    public const int DefaultMaxTasks = 1;

    public static readonly WSHttpBinding WsHttpBinding = new(SecurityMode.Transport)
    {
        Security = { Transport = { ClientCredentialType = HttpClientCredentialType.Basic } },
        MaxReceivedMessageSize = MaxMessageSize,
        ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
        {
            MaxStringContentLength = int.MaxValue,
        },
        ReceiveTimeout = DefaultTimeout,
        SendTimeout = DefaultTimeout,
    };
}