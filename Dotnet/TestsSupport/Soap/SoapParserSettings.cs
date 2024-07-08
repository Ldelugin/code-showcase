using System.Collections.Generic;

namespace TestsSupport.Soap;

/// <summary>
/// Settings to use when parsing a soap message.
/// </summary>
public class SoapParserSettings
{
    /// <summary>
    /// The default total number of attempts to shuffle an element.
    /// When <see cref="TotalShuffleAttempts"/> is set to something below 1, this value will be used.
    /// </summary>
    public const int DefaultTotalShuffleAttemptsLowerBounds = 10;

    /// <summary>
    /// The upper bounds of the total number of attempts to shuffle an element.
    /// </summary>
    public const int TotalShuffleAttemptsUpperBounds = 1000;

    /// <summary>
    /// Creates a new instance of <see cref="SoapParserSettings"/> for a soap 1.1 message. 
    /// </summary>
    public static SoapParserSettings Soap11 => new() { SoapVersion = SoapVersion.Soap11 };

    /// <summary>
    /// Creates a new instance of <see cref="SoapParserSettings"/> for a soap 1.2 message.
    /// </summary>
    public static SoapParserSettings Soap12 => new() { SoapVersion = SoapVersion.Soap12 };

    /// <summary>
    /// The soap version to use when parsing the soap message.
    /// </summary>
    public SoapVersion SoapVersion { get; set; } = SoapVersion.Soap12;

    /// <summary>
    /// Should the body content be omitted when parsing the soap message.
    /// </summary>
    /// <example>
    /// Example on how the soap message will look like when the body content is omitted.
    /// <code>
    /// &lt;soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope"&gt;
    ///     &lt;soap:Header /&gt;
    ///     &lt;soap:Body /&gt;
    /// &lt;/soap:Envelope&gt;
    /// </code>
    /// </example>
    public bool OmitBodyContent { get; set; }

    /// <summary>
    /// Should the body content be partially omitted when parsing the soap message.
    /// The inner text of the body element will be omitted.
    /// </summary>
    /// <example>
    /// Example on how the soap message will look like when the body content is omitted.
    /// <code>
    /// &lt;soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope"&gt;
    ///     &lt;soap:Header /&gt;
    ///     &lt;soap:Body&gt;
    ///         &lt;ser:EnqueuePassages /&gt;
    ///     &lt;/soap:Body&gt;
    /// &lt;/soap:Envelope&gt;
    /// </code>
    /// </example>
    public bool PartiallyOmitBodyContent { get; set; }

    /// <summary>
    /// A dictionary of overrides to use when parsing the soap message. 
    /// </summary>
    /// <remarks>
    /// The key is the XPath to the element and the value is the value to set the element to.
    /// Note: that you need to include the namespace prefix in the XPath.
    /// </remarks>
    /// <example>
    /// Select the attribute 'Workflow' of the first 'Passage' element and set it to 'a:something-else'.
    /// Notice that the index is 1 based and not 0 based.
    /// <code>
    /// //ser:Passage[@Workflow='NORMAL'])[1]", "a:something-else"
    /// </code>
    /// </example>
    public Dictionary<string, string> Overrides { get; set; }

    /// <summary>
    /// A list of elements to shuffle when parsing the soap message.
    /// </summary>
    /// <remarks>
    /// Use the XPath to define the element that needs to be shuffled.
    /// Note: that you need to include the namespace prefix in the XPath.
    /// </remarks>
    /// <example>
    /// Shuffle all the elements 'Passage' and 'Image'.
    /// <code>
    /// "//ser:Passage", "//pas:Image"
    /// </code>
    /// Shuffle the first 'Passage' element and the second 'Image' element of the second passage.
    /// <code>
    /// "//ser:Passage[1]", "//ser:Passage[2]/pas:Image[2]"
    /// </code>
    /// </example>
    public List<string> ElementsToShuffle { get; set; }

    /// <summary>
    /// The total number of attempts to shuffle an element.
    /// </summary>
    public int TotalShuffleAttempts { get; set; } = DefaultTotalShuffleAttemptsLowerBounds;

    /// <summary>
    /// A list of elements to add when parsing the soap message.
    /// </summary>
    /// <remarks>
    /// The key is the XPath to the element and the value is the value to set the element to.
    /// Note: that you need to include the namespace prefix in the XPath.
    /// </remarks>
    /// <example>
    /// Add an element 'AdditionalElement' with the value 'AdditionalElementValue' to each 'Passage' element.
    /// <code>
    /// "//ser:Passage", Value: { "AdditionalElement", "AdditionalElementValue" }
    /// </code>
    /// </example>
    public Dictionary<string, Dictionary<string, string>> AdditionalElementsToAdd { get; set; }

    /// <summary>
    /// A list of attributes to add when parsing the soap message.
    /// </summary>
    /// <remarks>
    /// The key is the XPath to the element and the value is a dictionary of attributes to add.
    /// Note: that you need to include the namespace prefix in the XPath.
    /// </remarks>
    /// <example>
    /// Add an attribute 'AdditionalAttribute' with the value 'AdditionalAttributeValue' to each 'Passage' element.
    /// <code>
    /// "//ser:Passage", { "AdditionalAttribute", "AdditionalAttributeValue" }
    /// </code>
    /// </example>
    public Dictionary<string, Dictionary<string, string>> AdditionalAttributesToAdd { get; set; }
}