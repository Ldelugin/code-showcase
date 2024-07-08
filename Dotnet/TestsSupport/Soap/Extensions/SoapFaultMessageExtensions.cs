using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsSupport.Soap.Extensions;

/// <summary>
/// Contains extension methods for <see cref="SoapFaultMessage"/>.
/// </summary>
public static class SoapFaultMessageExtensions
{
    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> is not null.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage IsNotNull(this SoapFaultMessage message)
    {
        Assert.IsNotNull(message);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has 's:Sender' as fault code.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasSenderAsCode(this SoapFaultMessage message)
    {
        Assert.AreEqual("s:Sender", message.Code);
        return message;
    }

    public static SoapFaultMessage HasExpectedCode(this SoapFaultMessage message, SoapVersion soapVersion)
    {
        Assert.AreEqual(soapVersion == SoapVersion.Soap12 ? "s:Receiver" : "s:Server", message.Code);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has '400_BAD_REQUEST' as fault reason.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasBasRequestAsReason(this SoapFaultMessage message)
    {
        Assert.AreEqual("400_BAD_REQUEST", message.Reason);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has '500_INTERNAL_SERVER_ERROR' as fault reason.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasInternalServerErrorAsReason(this SoapFaultMessage message)
    {
        Assert.AreEqual("500_INTERNAL_SERVER_ERROR", message.Reason);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has '401_UNAUTHORIZED' as fault reason.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasUnauthorizedAsReason(this SoapFaultMessage message)
    {
        Assert.AreEqual("401_UNAUTHORIZED", message.Reason);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has no detail message.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasNoInvalidDetailMessage(this SoapFaultMessage message)
    {
        Assert.IsNull(message.DetailMessage);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has  a detail message.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <param name="expectedMessage"></param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasInvalidDetailMessage(this SoapFaultMessage message, string expectedMessage)
    {
        Assert.AreEqual(expectedMessage, message.DetailMessage);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has a default detail message.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasDefaultInvalidDetailMessage(this SoapFaultMessage message) =>
        message.HasInvalidDetailMessage("Visit http://localhost/-- redacted -- for more information.");

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has no reasons.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasNoReasons(this SoapFaultMessage message)
    {
        Assert.AreEqual(expected: 0, message.Reasons.Count);
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has a reason that matches the provided predicate.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <param name="predicate"></param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasReason(this SoapFaultMessage message, Func<string, bool> predicate)
    {
        Assert.IsTrue(message.Reasons.Any(predicate));
        return message;
    }

    /// <summary>
    /// Asserts if the provided <see cref="SoapFaultMessage"/> has a reason that matches the provided expected reason.
    /// </summary>
    /// <param name="message">Instance of <see cref="SoapFaultMessage"/>.</param>
    /// <param name="expectedReason"></param>
    /// <returns>The current instance of <see cref="SoapFaultMessage"/>.</returns>
    public static SoapFaultMessage HasReason(this SoapFaultMessage message, string expectedReason)
    {
        Assert.IsTrue(message.Reasons.Contains(expectedReason));
        return message;
    }
}