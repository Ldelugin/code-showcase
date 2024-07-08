using System.Collections.Generic;
using System.Xml.Linq;

namespace TestsSupport.Soap;

/// <summary>
/// Record holding the root element, the root namespace and the namespace attributes.
/// </summary>
/// <param name="RootElement">The root element.</param>
/// <param name="RootNamespace">The root namespace.</param>
/// <param name="NamespaceAttributes">A list with the namespace attributes.</param>
public record ParsingContext(XElement RootElement, XNamespace RootNamespace, List<XAttribute> NamespaceAttributes);