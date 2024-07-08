using System;
using System.ServiceModel;
using System.Xml;

namespace -- redacted --;

/// <summary>
/// Implementation for the <see cref="IBindingFactory"/>.
/// </summary>
public class BindingFactory : IBindingFactory
{
    /// <summary>
    /// The default timeout for the bindings ReceiveTimeout and SendTimeout.
    /// The ReceiveTimeout specifies how long a connection can remain inactive,
    /// during which no application messages are received, before it is dropped.
    /// The SendTimeout specifies the time a write operation has to complete
    /// before the transport raises an exception.
    /// </summary>
    public TimeSpan DefaultTimeout => TimeSpan.FromMinutes(value: 10);

    /// <summary>
    /// Get an instance of an interoperable binding that supports distributed
    /// transactions and secure, reliable sessions. 
    /// </summary>
    /// <returns>
    /// Returns an instance of <see cref="WSHttpBinding"/>.
    /// </returns>
    public WSHttpBinding CreateWsHttpBinding()
    {
        return new WSHttpBinding
        {
            Security =
            {
                Mode = SecurityMode.Transport,
                Message = { ClientCredentialType = MessageCredentialType.None },
                Transport =
                {
                    ClientCredentialType = HttpClientCredentialType.Basic,
                    ProxyCredentialType = HttpProxyCredentialType.None
                }
            },
            MaxBufferPoolSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue,
            ReaderQuotas = XmlDictionaryReaderQuotas.Max,
            ReceiveTimeout = this.DefaultTimeout,
            SendTimeout = this.DefaultTimeout
        };
    }

    /// <summary>
    /// Get an instance of a binding for the communication with ASMX-based Web services and clients and other
    /// services that conform to the WS-I Basic Profile 1.1.
    /// </summary>
    /// <returns>
    /// Returns an instance of <see cref="BasicHttpBinding"/>.
    /// </returns>
    public BasicHttpBinding CreateBasicHttpBinding()
    {
        return new BasicHttpBinding
        {
            MaxBufferPoolSize = int.MaxValue,
            MaxBufferSize = int.MaxValue,
            MaxReceivedMessageSize = int.MaxValue,
            ReaderQuotas = XmlDictionaryReaderQuotas.Max,
            ReceiveTimeout = this.DefaultTimeout,
            SendTimeout = this.DefaultTimeout,
            AllowCookies = true
        };
    }
}
