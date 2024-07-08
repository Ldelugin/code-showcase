using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace -- redacted --;

/// <summary>
/// Exception to be thrown when a bad request is made in one of the API calls.
/// </summary>
[Serializable]
public class BadRequestFaultException : FaultException<InvalidDetail>
{
    /// <summary>
    /// Creates instance of <see cref="BadRequestFaultException"/>.
    /// </summary>
    /// <param name="invalidDetail">
    /// Provide an <see cref="InvalidDetail"/> to describe why the request 
    /// was invalid.
    /// </param>
    public BadRequestFaultException(InvalidDetail invalidDetail) : base(
              invalidDetail,
              new FaultReason("400_BAD_REQUEST"),
              new FaultCode("Sender"),
              "")
    {
    }

    /// <inheritdoc />
    protected BadRequestFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        // Nothing to do.
    }
}
