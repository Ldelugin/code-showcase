using System.Collections.Generic;
using System.Xml;

namespace TestsSupport.Soap;

/// <summary>
/// Represents a soap fault message.
/// </summary>
public class SoapFaultMessage
{
    /// <summary>
    /// The fault code.
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// The fault reason.
    /// </summary>
    public string Reason { get; private set; }

    /// <summary>
    /// The detail message.
    /// </summary>
    public string DetailMessage { get; private set; }

    /// <summary>
    /// The reasons in the detail message.
    /// </summary>
    public List<string> Reasons { get; private set; }

    /// <summary>
    /// Parses the provided text to a <see cref="SoapFaultMessage"/>.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="soapVersion">The soap version of the message.</param>
    /// <returns>
    /// A <see cref="SoapFaultMessage"/> that contains the parsed message.
    /// </returns>
    /// <exception cref="XmlException">
    /// Thrown when the text to parse does not represents a soap fault message.
    /// </exception>
    public static SoapFaultMessage Parse(string text, SoapVersion soapVersion)
    {
        // Create an XmlDocument.
        XmlDocument doc = new();

        // Load the document with the SOAP message.
        doc.LoadXml(text);

        // Get the fault element.
        var faultElement = (doc["s:Envelope"]?["s:Body"]?["s:Fault"])
                           ?? throw new XmlException($"$The provided text is not a soap fault message.\nThe text is:\n{text}");

        // Get the fault code, reason and detail message.
        var codeText = soapVersion == SoapVersion.Soap12
            ? faultElement["s:Code"]?["s:Value"]?.InnerText
            : faultElement["faultcode"]?.InnerText;
        var reasonText = soapVersion == SoapVersion.Soap12
            ? faultElement["s:Reason"]?["s:Text"]?.InnerText
            : faultElement["faultstring"]?.InnerText;
        var detailName = soapVersion == SoapVersion.Soap12 ? "s:Detail" : "detail";
        var invalidDetail = faultElement[detailName]?["InvalidDetail"];
        var detailMessageText = invalidDetail?["Message"]?.InnerText;
        var reasons = new List<string>();
        var reasonsNode = invalidDetail?["Reasons"];
        if (reasonsNode != null)
        {
            foreach (XmlNode reasonNode in reasonsNode.ChildNodes)
            {
                reasons.Add(reasonNode.InnerText);
            }
        }

        return new SoapFaultMessage
        {
            Code = codeText,
            Reason = reasonText,
            DetailMessage = detailMessageText,
            Reasons = reasons
        };
    }
}