using System;
using System.ServiceModel;

namespace -- redacted --;

/// <summary>
/// Exposes methods to return a <see cref="WSHttpBinding"/> or <see cref="BasicHttpBinding"/>.
/// </summary>
public interface IBindingFactory
{
    /// <summary>
    /// The default timeout for the bindings ReceiveTimeout and SendTimeout.
    /// The ReceiveTimeout specifies how long a connection can remain inactive,
    /// during which no application messages are received, before it is dropped.
    /// The SendTimeout specifies the time a write operation has to complete
    /// before the transport raises an exception.
    /// </summary>
    TimeSpan DefaultTimeout { get; }

    /// <summary>
    /// Get an instance of an interoperable binding that supports distributed
    /// transactions and secure, reliable sessions.
    /// </summary>
    /// <returns>Returns a new instance of <see cref="WSHttpBinding"/>.</returns>
    WSHttpBinding CreateWsHttpBinding();

    /// <summary>
    /// Get an instance of a binding for the communication with ASMX-based Web services and clients and other
    /// services that conform to the WS-I Basic Profile 1.1.
    /// </summary>
    /// <returns>Returns a new instance of <see cref="BasicHttpBinding"/>.</returns>
    BasicHttpBinding CreateBasicHttpBinding();
}
