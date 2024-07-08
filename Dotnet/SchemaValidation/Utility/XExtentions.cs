using System;
using System.Linq;
using System.Xml.Linq;

namespace -- redacted --;

/// <summary>
/// See https://stackoverflow.com/questions/451950/get-the-xpath-to-an-xelement
/// </summary>
public static class XExtentions
{
    public static string getAbsoluteXPath(this XAttribute attribute)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        return GetAbsoluteXPath(attribute.Parent) + "/@" + attribute.Name;
    }
    /// <summary>
    /// Get the absolute XPath to a given XElement
    /// (e.g. "/people/person[6]/name[1]/last[1]").
    /// </summary>
    public static string GetAbsoluteXPath(this XElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        static string RelativeXPath(XElement e)
        {
            var index = e.IndexPosition();
            var namespacePrefix = e.GetPrefixOfNamespace(e.Name.Namespace);
            var name = (namespacePrefix != null ? namespacePrefix + ":" : "") +
                       e.Name.LocalName;

            // If the element is the root, no index is required

            return index == -1 ? "/" + name : $"/{name}[{index}]";
        }

        var ancestors = from e in element.Ancestors()
                        select RelativeXPath(e);

        return string.Concat(ancestors.Reverse().ToArray()) +
               RelativeXPath(element);
    }

    /// <summary>
    /// Get the index of the given XElement relative to its
    /// siblings with identical names. If the given element is
    /// the root, -1 is returned.
    /// </summary>
    /// <param name="element">
    /// The element to get the index of.
    /// </param>
    public static int IndexPosition(this XElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (element.Parent == null || element.Parent.Elements(element.Name).Count() == 1)
        {
            return -1;
        }

        return element.Parent.Elements(element.Name).InDocumentOrder().ToList().IndexOf(element) + 1;
    }
}
