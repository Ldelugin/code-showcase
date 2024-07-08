using System.Collections.Generic;

namespace TestsSupport.Soap;

/// <summary>
/// Record holding an XPath and a value.
/// </summary>
/// <param name="XPath">The XPath to the element.</param>
/// <param name="Value">The value to set the element to.</param>
public record XPathWithValue(string XPath, string Value)
{
    /// <summary>
    /// Implicitly converts an <see cref="XPathWithValue"/> to a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="xPathWithValue">
    /// The <see cref="XPathWithValue"/> to convert to a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey,TValue}"/> that contains the XPath and the value.
    /// </returns>
    public static implicit operator KeyValuePair<string, string>(XPathWithValue xPathWithValue)
    {
        return new KeyValuePair<string, string>(xPathWithValue.XPath, xPathWithValue.Value);
    }
}