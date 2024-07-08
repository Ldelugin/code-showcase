using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace -- redacted --;

/// <summary>
/// Exception to be thrown when a something unforeseen happened at the receive side.
/// </summary>
[Serializable]
public class InternalServerErrorFaultException : FaultException<InvalidDetail>
{
    /// <summary>
    /// Instantiates a new instance of <see cref="InternalServerErrorFaultException"/>.
    /// </summary>
    public InternalServerErrorFaultException() : base(
        new InvalidDetail { Reasons = ["Internal server error"] },
        new FaultReason("500_INTERNAL_SERVER_ERROR"),
        new FaultCode("Receiver"),
        "")
    {
    }

    /// <inheritdoc />
    protected InternalServerErrorFaultException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // Nothing to do.
    }
}
