using System.Xml.Linq;

namespace TestsSupport.Soap;

/// <summary>
/// Contains constants for SOAP.
/// </summary>
public static class SoapConstants
{
    /// <summary>
    /// The SOAP 1.2 namespace.
    /// </summary>
    public static readonly XNamespace Soap12Namespace = "http://www.w3.org/2003/05/soap-envelope";

    /// <summary>
    /// The SOAP 1.1 namespace.
    /// </summary>
    public static readonly XNamespace Soap11Namespace = "http://schemas.xmlsoap.org/soap/envelope/";

    /// <summary>
    /// The XML schema instance namespace. 
    /// </summary>
    public static readonly XNamespace XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>
    /// The array serialization namespace.
    /// </summary>
    public static readonly XNamespace SerializationArrayNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

    /// <summary>
    /// The SOAP 1.1 namespace prefix.
    /// </summary>
    public const string Soap11NamespacePrefix = "soapenv";

    /// <summary>
    /// The name of the SOAP envelope element.
    /// </summary>
    public const string SoapEnvelopeElementName = "Envelope";

    /// <summary>
    /// The name of the SOAP header element.
    /// </summary>
    public const string SoapHeaderElementName = "Header";

    /// <summary>
    /// The name of the SOAP body element.
    /// </summary>
    public const string SoapBodyElementName = "Body";
}