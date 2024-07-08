using System.Xml.Serialization;

namespace -- redacted --;

/// <summary>
/// InvalidDetail is used with the <see cref="BadRequestFaultException"/> to describe 
/// why the request was invalid.
/// </summary>
[XmlRoot(ElementName = "Reasons")]
public class InvalidDetail
{
    /// <summary>
    /// An message element, to provide more information.
    /// </summary>
    [XmlElement(ElementName = "Message", Order = 0)]
    public string Message { get; set; }

    /// <summary>
    /// An array with reasons describing why the request was invalid.
    /// </summary>
    [XmlElement(ElementName = "Reason", Order = 1)]
    public string[] Reasons { get; set; }
}
