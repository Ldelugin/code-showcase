using System.Collections.Generic;

namespace TestsSupport.Soap;

/// <summary>
/// Record holding an XPath and a dictionary of name value pairs.
/// </summary>
/// <param name="XPath">The XPath to the element.</param>
/// <param name="NameValuePairs">The dictionary of name value pairs.</param>
public record XPathWithNameValuePairs(string XPath, Dictionary<string, string> NameValuePairs)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XPathWithNameValuePairs"/> class.
    /// </summary>
    /// <param name="xPath">The XPath to the element.</param>
    public XPathWithNameValuePairs(string xPath) : this(xPath, [])
    {
    }

    /// <summary>
    /// Adds a name value pair to the dictionary.
    /// </summary>
    /// <param name="name">The name to add.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>
    /// Returns the current instance of <see cref="XPathWithNameValuePairs"/>.
    /// </returns>
    public XPathWithNameValuePairs Add(string name, string value)
    {
        this.NameValuePairs.Add(name, value);
        return this;
    }

    /// <summary>
    /// Implicitly converts an <see cref="XPathWithNameValuePairs"/> to a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="xPathWithNameValuePairs">
    /// The <see cref="XPathWithNameValuePairs"/> to convert to a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey,TValue}"/> that contains the XPath and the dictionary of name value pairs.
    /// </returns>
    public static implicit operator KeyValuePair<string, Dictionary<string, string>>(XPathWithNameValuePairs xPathWithNameValuePairs)
    {
        return new KeyValuePair<string, Dictionary<string, string>>(xPathWithNameValuePairs.XPath,
            xPathWithNameValuePairs.NameValuePairs);
    }
}