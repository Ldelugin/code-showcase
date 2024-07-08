using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace TestsSupport.Soap.Extensions;

/// <summary>
/// Contains extension methods for XML related operations.
/// </summary>
public static partial class XmlExtensions
{
    /// <summary>
    /// Parse the provided object into a SOAP document.
    /// </summary>
    /// <param name="instance">The object to parse.</param>
    /// <param name="settings">The parsing settings.</param>
    /// <returns>The parsed soap encoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="instance"/> does not have a <see cref="MessageContractAttribute"/>.</exception>
    public static string AsSoap(this object instance, SoapParserSettings settings)
    {
        // If settings weren't received, use default settings
        settings ??= new SoapParserSettings();

        // Ensure that the instance parameter isn't null
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var instanceType = instance.GetType();
        var messageContractAttribute = instanceType.GetCustomAttribute<MessageContractAttribute>()
                                                                             ?? throw new ArgumentException($"The type {instanceType.FullName} does not have a {nameof(MessageContractAttribute)}.");
        XNamespace serviceNamespace = messageContractAttribute.WrapperNamespace;

        // Generate the SOAP document
        var document = settings.CreateSoapDocument(namespaceAttributes =>
        {
            namespaceAttributes.Add(serviceNamespace.ConvertToXAttribute());
            var bodyContent = new XElement(serviceNamespace + messageContractAttribute.WrapperName);
            if (settings.PartiallyOmitBodyContent)
            {
                return bodyContent;
            }

            var context = new ParsingContext(bodyContent, serviceNamespace, namespaceAttributes);
            context.ParseMessageBodyBasedBodyContent(instance, instanceType);
            return bodyContent;
        });

        // Transform the document based on the settings
        _ = document.Transform(settings);

        // Return the SOAP document as a string
        return document.ToString();
    }

    /// <summary>
    /// Parse the selected operation of the provided object into a SOAP document.
    /// </summary>
    /// <param name="instance">The object to select the operation from.</param>
    /// <param name="operationSelector">The expression that selects the operation.</param>
    /// <param name="settings">The parsing settings.</param>
    /// <typeparam name="T">The type of <paramref name="instance"/>.</typeparam>
    /// <returns>The parsed soap encoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="instance"/> does not have a <see cref="ServiceContractAttribute"/> or
    /// the selected operation does not have a <see cref="OperationContractAttribute"/>.
    /// </exception>
    public static string AsSoap<T>(this T instance, Expression<Func<T, object>> operationSelector, SoapParserSettings settings)
    {
        // If settings weren't received, use default settings
        settings ??= new SoapParserSettings();

        // Ensure that the instance parameter isn't null
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var instanceType = instance.GetType();
        var serviceContract = instanceType.GetCustomAttribute<ServiceContractAttribute>()
                              ?? throw new ArgumentException($"The type {instanceType.FullName} does not have a {nameof(ServiceContractAttribute)}.");
        XNamespace serviceNamespace = serviceContract.Namespace;

        var methodCallExpression = (MethodCallExpression)operationSelector.Body;
        var method = methodCallExpression.Method;

        var operationContract = method.GetCustomAttribute<OperationContractAttribute>()
                                ?? throw new ArgumentException($"The type {instanceType.FullName} does not have a {nameof(OperationContractAttribute)}.");

        // Generate the SOAP document
        var document = settings.CreateSoapDocument(namespaceAttributes =>
        {
            namespaceAttributes.Add(serviceNamespace.ConvertToXAttribute());
            var name = operationContract.Name ?? method.Name;
            var bodyContent = new XElement(serviceNamespace + name);
            if (settings.PartiallyOmitBodyContent)
            {
                return bodyContent;
            }

            var context = new ParsingContext(bodyContent, serviceNamespace, namespaceAttributes);
            context.ParseOperationBasedBodyContent(methodCallExpression);
            return bodyContent;
        });

        // Transform the document based on the settings
        _ = document.Transform(settings);

        // Return the SOAP document as a string
        return document.ToString();
    }

    /// <summary>
    /// Parse the selected operation of the provided object into a SOAP document.
    /// </summary>
    /// <param name="instance">The object to select the operation from.</param>
    /// <param name="operationSelector">The expression that selects the operation.</param>
    /// <param name="settings">The parsing settings.</param>
    /// <typeparam name="T">The type of <paramref name="instance"/>.</typeparam>
    /// <returns>The parsed soap encoded string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="instance"/> does not have a <see cref="ServiceContractAttribute"/> or
    /// the selected operation does not have a <see cref="OperationContractAttribute"/>.
    /// </exception>
    public static string AsSoap<T>(this T instance, Expression<Action<T>> operationSelector, SoapParserSettings settings)
    {
        // If settings weren't received, use default settings
        settings ??= new SoapParserSettings();

        // Ensure that the instance parameter isn't null
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        var instanceType = instance.GetType();
        var serviceContract = instanceType.GetCustomAttribute<ServiceContractAttribute>()
                              ?? throw new ArgumentException($"The type {instanceType.FullName} does not have a {nameof(ServiceContractAttribute)}.");
        XNamespace serviceNamespace = serviceContract.Namespace;

        var methodCallExpression = (MethodCallExpression)operationSelector.Body;
        var method = methodCallExpression.Method;

        var operationContract = method.GetCustomAttribute<OperationContractAttribute>()
                                ?? throw new ArgumentException($"The type {instanceType.FullName} does not have a {nameof(OperationContractAttribute)}.");

        // Generate the SOAP document
        var document = settings.CreateSoapDocument(namespaceAttributes =>
        {
            namespaceAttributes.Add(serviceNamespace.ConvertToXAttribute());
            var name = operationContract.Name ?? method.Name;
            var bodyContent = new XElement(serviceNamespace + name);
            if (settings.PartiallyOmitBodyContent)
            {
                return bodyContent;
            }

            var context = new ParsingContext(bodyContent, serviceNamespace, namespaceAttributes);
            context.ParseOperationBasedBodyContent(methodCallExpression);
            return bodyContent;
        });

        // Transform the document based on the settings
        _ = document.Transform(settings);

        // Return the SOAP document as a string
        return document.ToString();
    }

    /// <summary>
    /// Create the SOAP document.
    /// </summary>
    /// <param name="settings">The parsing settings.</param>
    /// <param name="bodyContentBuilder">Func to build the body content.</param>
    /// <returns>a new instance of <see cref="XDocument"/>.</returns>
    private static XDocument CreateSoapDocument(this SoapParserSettings settings, Func<List<XAttribute>, XElement> bodyContentBuilder)
    {
        var soapNamespace = settings.SoapVersion.ToSoapNamespace();
        var namespaceAttribute = settings.SoapVersion.ConvertToXAttribute();
        var namespaceAttributes = new List<XAttribute> { namespaceAttribute };
        var document = new XDocument();
        var soapBody = new XElement(soapNamespace + SoapConstants.SoapBodyElementName);

        if (!settings.OmitBodyContent && bodyContentBuilder != null)
        {
            var bodyContent = bodyContentBuilder(namespaceAttributes);
            soapBody.Add(bodyContent);
        }

        var soapEnvelope = new XElement(soapNamespace + SoapConstants.SoapEnvelopeElementName, namespaceAttributes.DistinctBy(x => x.Name));
        soapEnvelope.Add(new XElement(soapNamespace + SoapConstants.SoapHeaderElementName));
        soapEnvelope.Add(soapBody);
        document.Add(soapEnvelope);
        return document;
    }

    /// <summary>
    /// Parse the body content of the message based on the <see cref="MessageBodyMemberAttribute"/> attributes.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="instance">The object to parse.</param>
    /// <param name="instanceType">The type of the object to parse.</param>
    private static void ParseMessageBodyBasedBodyContent(this ParsingContext context, object instance, Type instanceType)
    {
        foreach (var bodyMember in instanceType.GetProperties(p => p.GetCustomAttribute<MessageBodyMemberAttribute>() != null))
        {
            var attr = bodyMember.GetCustomAttribute<MessageBodyMemberAttribute>()!;
            var value = bodyMember.GetValue(instance);
            var valueType = bodyMember.PropertyType;

            if (valueType.IsPrimitive || valueType == typeof(string))
            {
                var bodyMemberElement = new XElement(context.RootNamespace + attr.Name);
                if (value != null)
                {
                    bodyMemberElement.SetValue(value);
                }
                context.RootElement.Add(bodyMemberElement);
                continue;
            }

            context.ParseComplexType(attr.Name, value, valueType);
        }
    }

    /// <summary>
    /// Parse the body content of the message based on the selected operation.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="methodCallExpression">The expression representing the selected operation.</param>
    private static void ParseOperationBasedBodyContent(this ParsingContext context, MethodCallExpression methodCallExpression)
    {
        var method = methodCallExpression.Method;
        var parameters = method.GetParameters();
        var arguments = methodCallExpression.Arguments;
        for (var i = 0; i < arguments.Count; i++)
        {
            var parameter = parameters[i];
            var argument = arguments[i];

            // compile the argument expression to get the value
            var value = Expression.Lambda(argument).Compile().DynamicInvoke();
            var valueType = parameter.ParameterType;

            // No need to do extra parsing if the type is primitive or string.
            // Because the string IEnumerable<char> it will be seen as a collection and will be parsed as such, so to prevent that it's checked here.
            if (valueType.IsPrimitive || valueType == typeof(string))
            {
                var bodyMemberElement = new XElement(context.RootNamespace + parameter.Name);
                if (value != null)
                {
                    bodyMemberElement.SetValue(value);
                }
                context.RootElement.Add(bodyMemberElement);
                continue;
            }

            // If the parameter has XmlArrayAttribute or XmlArrayItemAttribute, parse it as an xml array.
            if (parameter.TryGetXmlArrayAttributes(out var arrayAttribute, out var arrayItemAttribute))
            {
                context.ParseXmlArray(parameter.Name, value, arrayAttribute, arrayItemAttribute);
                continue;
            }

            // The type is either an array/collection or a class with it's own members to parse.
            context.ParseComplexType(parameter.Name, value, valueType);
        }
    }

    /// <summary>
    /// Parse the provided object as a complex type.
    /// The type is either an array/collection or a class with it's own members to parse.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The object that needs to be parsed.</param>
    /// <param name="valueType">The type of the object.</param>
    private static void ParseComplexType(this ParsingContext context, string name, object value, Type valueType)
    {
        if (valueType.TryGetXmlArrayAttributes(out var arrayAttribute, out var arrayItemAttribute))
        {
            context.ParseXmlArray(name, value, arrayAttribute, arrayItemAttribute);
            return;
        }

        if (valueType.IsGenericCollection() && valueType != typeof(string))
        {
            context.ParseGenericCollection(name, value);
            return;
        }

        if (!valueType.TryGetXNamespaceAndXAttribute(out var ns, out var xAttribute))
        {
            return;
        }

        var element = xAttribute.CreateXElement(context, name);
        context.ParseObjectProperties(value.GetType(), value, element, ns);
        context.RootElement.Add(element);
    }

    /// <summary>
    /// Parse the provided object as an XML array.
    /// </summary>
    /// <remarks>
    /// The usage of XmlArrayAttribute and XmlArrayItemAttribute is as follows
    ///<code>
    /// public class Request
    /// {
    ///     [XmlArray("Items")]
    ///     [XmlArrayItem("Item")]
    ///     public List&lt;Item&gt; Items { get; set; }
    /// }
    ///</code>
    /// And how it would be parsed:
    /// <code>
    /// &lt;Request&gt;
    ///     &lt;Items&gt;
    ///         &lt;Item&gt;
    ///             &lt;Name&gt;Item 1&lt;/Name&gt;
    ///         &lt;/Item&gt;
    ///     &lt;/Items&gt;
    /// &lt;/Request&gt;
    ///</code>
    /// </remarks>
    /// <param name="context">The parsing context.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The object that needs to be parsed.</param>
    /// <param name="arrayAttribute">Instance of <see cref="XmlArrayAttribute"/>, can be null.</param>
    /// <param name="arrayItemAttribute">Instance of <see cref="XmlArrayItemAttribute"/>, can be null.</param>
    private static void ParseXmlArray(this ParsingContext context, string name, object value, XmlArrayAttribute arrayAttribute, XmlArrayItemAttribute arrayItemAttribute)
    {
        var parent = context.RootElement;
        // It might be possible that the XmlArrayAttribute is null and thus is not wrapped inside an array element.
        if (arrayAttribute != null)
        {
            var arrayName = string.IsNullOrWhiteSpace(arrayAttribute.ElementName) ? name : arrayAttribute.ElementName;
            var arrayElement = new XElement(context.RootNamespace + arrayName);
            context.RootElement.Add(arrayElement);
            parent = arrayElement;
        }

        // Get the generic collection and check if it's null or empty.
        var collection = value.GetGenericCollection();
        if (collection == null || collection.Count == 0)
        {
            return;
        }

        // Iterate over the collection and parse each item.
        foreach (var item in collection)
        {
            var itemType = item.GetType();
            var itemName = string.IsNullOrWhiteSpace(arrayItemAttribute.ElementName) ? itemType.Name : arrayItemAttribute.ElementName;
            var element = new XElement(context.RootNamespace + itemName);
            context.ParseObjectProperties(itemType, item, element, context.RootNamespace);
            parent.Add(element);
        }
    }

    /// <summary>
    /// Parse the provided object as a generic collection.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The object that needs to be parsed.</param>
    private static void ParseGenericCollection(this ParsingContext context, string name, object value)
    {
        // Get the generic collection and check if it's null or empty.
        var collection = value.GetGenericCollection();
        if (collection == null || collection.Count == 0)
        {
            return;
        }

        // Get the namespace and attribute of the first item in the collection.
        var firstItem = collection.FirstOrDefault();
        if (!firstItem.TryGetXNamespaceAndXAttribute(out var ns, out var xAttribute))
        {
            return;
        }

        // Iterate over the collection and parse each item.
        context.NamespaceAttributes.Add(xAttribute);
        foreach (var item in collection)
        {
            var element = xAttribute.CreateXElement(context, name);
            context.ParseObjectProperties(item.GetType(), item, element, ns);
            context.RootElement.Add(element);
        }
    }

    /// <summary>
    /// Parse the properties of the provided object.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="itemType">The type of the object that needs to be parsed.</param>
    /// <param name="item">The object that needs to be parsed.</param>
    /// <param name="rootElement">The root element.</param>
    /// <param name="ns">The namespace.</param>
    private static void ParseObjectProperties(this ParsingContext context, Type itemType, object item, XElement rootElement, XNamespace ns)
    {
        foreach (var prop in itemType.GetProperties())
        {
            var value = prop.GetValue(item);
            if (rootElement.TrySetAttributeValueFromProperty(prop, value))
            {
                continue;
            }

            _ = context.TrySetElementAttributeFromProperty(rootElement, ns, prop, value);
        }
    }

    /// <summary>
    /// Try to set the attribute value from the property.
    /// </summary>
    /// <param name="rootElement">The root element of the property.</param>
    /// <param name="prop">The <see cref="PropertyInfo"/>.</param>
    /// <param name="value">The object that needs to be parsed.</param>
    /// <returns><c>true</c> if the property has an <see cref="XmlAttributeAttribute"/>, otherwise <c>false</c></returns>
    private static bool TrySetAttributeValueFromProperty(this XElement rootElement, PropertyInfo prop, object value)
    {
        var attributeAttribute = prop.GetCustomAttribute<XmlAttributeAttribute>();
        if (attributeAttribute == null)
        {
            return false;
        }

        if (prop.PropertyType.IsEnum)
        {
            value = value.GetEnumName();
        }

        rootElement.SetAttributeValue(attributeAttribute.AttributeName, value);
        return true;
    }

    /// <summary>
    /// Try to set the element attribute from the property.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="rootElement">The root element of the property.</param>
    /// <param name="ns">The namespace.</param>
    /// <param name="prop">The <see cref="PropertyInfo"/>.</param>
    /// <param name="value">The object that needs to be parsed.</param>
    /// <returns><c>true</c> if the property has an <see cref="XmlElementAttribute"/>, otherwise <c>false</c></returns>
    private static bool TrySetElementAttributeFromProperty(this ParsingContext context, XElement rootElement, XNamespace ns, PropertyInfo prop, object value)
    {
        var elementAttribute = prop.GetCustomAttribute<XmlElementAttribute>();
        if (elementAttribute == null || value == null)
        {
            return false;
        }

        var name = string.IsNullOrWhiteSpace(elementAttribute.ElementName) ? prop.Name : elementAttribute.ElementName;
        var propertyElement = new XElement(ns + name);
        var propertyType = prop.PropertyType;
        var needsComplexParsing = value.IsXmlType() && !propertyType.IsValueType;

        if (needsComplexParsing)
        {
            ParseComplexType(new ParsingContext(rootElement, ns, context.NamespaceAttributes), name, value, propertyType);
        }
        else
        {
            propertyElement.SetValue(value);
            rootElement.Add(propertyElement);

            if (elementAttribute.IsNullable)
            {
                rootElement.Add(new XElement(propertyElement.Name + "Specified", content: true));
            }
        }

        return true;
    }

    /// <summary>
    /// Creating an XML element.
    /// </summary>
    /// <param name="xAttribute">The <see cref="XAttribute"/>.</param>
    /// <param name="context">The parsing context.</param>
    /// <param name="name">The name of the element.</param>
    /// <returns>The <see cref="XElement"/>.</returns>
    private static XElement CreateXElement(this XAttribute xAttribute, ParsingContext context, string name) =>
        xAttribute != null ? new XElement(context.RootNamespace + name) : new XElement(name);

    /// <summary>
    /// This method is used to provide a namespace manager for an XDocument
    /// </summary>
    /// <param name="document">The document to create the namespace manager for.</param>
    /// <returns>Returns the <see cref="XmlNamespaceManager"/> for the document.</returns>
    private static XmlNamespaceManager GetXmlNamespaceManager(this XDocument document)
    {
        var namespaceManager = new XmlNamespaceManager(new NameTable());
        var namespaceAttributes = document?.Root?.Attributes() ?? [];
        foreach (var namespaceAttribute in namespaceAttributes)
        {
            namespaceManager.AddNamespace(namespaceAttribute.Name.LocalName, namespaceAttribute.Value);
        }
        return namespaceManager;
    }

    /// <summary>
    /// Converts a given XNamespace into an XAttribute instance.
    /// </summary>
    /// <param name="ns">The XNamespace instance to convert.</param>
    /// <param name="prefixTotalLetters">Total letters to consider from the end segment of the namespace as prefix for the attribute.</param>
    /// <returns>An XAttribute instance built from the provided XNamespace.</returns>
    public static XAttribute ConvertToXAttribute(this XNamespace ns, int prefixTotalLetters = 3) =>
        ns.ConvertToXAttribute(ns.NamespaceName.GetFirstXLettersOfLastNamespacePart(prefixTotalLetters));

    /// <summary>
    /// Converts a given XNamespace into an XAttribute instance with a specific prefix.
    /// </summary>
    /// <param name="ns">The XNamespace instance to convert.</param>
    /// <param name="prefix">The prefix to use for the generated attribute.</param>
    /// <returns>An XAttribute instance built from the provided XNamespace and prefix.</returns>
    private static XAttribute ConvertToXAttribute(this XNamespace ns, string prefix) =>
        new(XNamespace.Xmlns + prefix, ns);

    /// <summary>
    /// Returns the first X letters from the last part of a given namespace.
    /// </summary>
    /// <param name="text">The namespace as a string.</param>
    /// <param name="totalLetters">Number of letters to consider.</param>
    /// <returns>First X letters from the last part of the namespace.</returns>
    private static string GetFirstXLettersOfLastNamespacePart(this string text, int totalLetters = 3)
    {
        var uri = new Uri(text);
        var lastSegment = uri.Segments.Last();
        return lastSegment[..(totalLetters <= lastSegment.Length ? totalLetters : lastSegment.Length)];
    }

    /// <summary>
    /// Converts a given SoapVersion to an equivalent XNamespace instance.
    /// </summary>
    /// <param name="soapVersion">The SoapVersion to convert.</param>
    /// <returns>An XNamespace instance equivalent to provided SoapVersion.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid SoapVersion is provided.</exception>
    public static XNamespace ToSoapNamespace(this SoapVersion soapVersion) =>
        soapVersion switch
        {
            SoapVersion.Soap11 => SoapConstants.Soap11Namespace,
            SoapVersion.Soap12 => SoapConstants.Soap12Namespace,
            _ => throw new ArgumentOutOfRangeException(nameof(soapVersion), soapVersion, message: null)
        };

    /// <summary>
    /// Converts a given SoapVersion to an equivalent XAttribute instance.
    /// </summary>
    /// <param name="soapVersion">The SoapVersion to convert.</param>
    /// <returns>An XAttribute instance equivalent to provided SoapVersion.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid SoapVersion is provided.</exception>
    public static XAttribute ConvertToXAttribute(this SoapVersion soapVersion)
    {
        var soapNamespace = soapVersion.ToSoapNamespace();
        return soapVersion switch
        {
            SoapVersion.Soap11 => soapNamespace.ConvertToXAttribute(SoapConstants.Soap11NamespacePrefix),
            SoapVersion.Soap12 => soapNamespace.ConvertToXAttribute(prefixTotalLetters: 4),
            _ => throw new ArgumentOutOfRangeException(nameof(soapVersion), soapVersion, message: null)
        };
    }

    /// <summary>
    /// Transforms the provided document based on the provided settings.
    /// </summary>
    /// <param name="document">The document to transform.</param>
    /// <param name="settings">Parsing settings.</param>
    /// <returns>The transformed document.</returns>
    public static XDocument Transform(this XDocument document, SoapParserSettings settings)
    {
        return document?.OverrideIfNeeded(settings)
            .AddAdditionalElementsIfNeeded(settings)
            .AddAdditionalAttributesIfNeeded(settings)
            .ShuffleIfNeeded(settings);
    }

    /// <summary>
    /// This method applies overrides from the settings to the document.
    /// </summary>
    /// <param name="document">The document to apply the overrides to.</param>
    /// <param name="settings">Parsing settings.</param>
    /// <returns>The transformed document.</returns>
    private static XDocument OverrideIfNeeded(this XDocument document, SoapParserSettings settings)
    {
        if (settings?.Overrides == null)
        {
            return document;
        }

        var namespaceManager = document.GetXmlNamespaceManager();

        foreach (var settingsOverride in settings.Overrides)
        {
            document.ApplyOverrideToDocument(settingsOverride, namespaceManager);
        }

        return document;
    }

    /// <summary>
    /// This method applies an override for a specific element in the XML document.
    /// </summary>
    /// <param name="document">
    /// The XML document.
    /// </param>
    /// <param name="settingsOverride">
    /// Key: XPath to the element to override.
    /// Value: The value to override the element with.
    /// </param>
    /// <param name="namespaceManager">The namespace manager.</param>
    private static void ApplyOverrideToDocument(this XNode document, KeyValuePair<string, string> settingsOverride,
        IXmlNamespaceResolver namespaceManager)
    {
        var xPath = settingsOverride.Key;
        var elements = document.XPathSelectElements(xPath, namespaceManager);
        var value = settingsOverride.Value;
        var nodeType = GetNodeTypeRegex().Match(value).Groups[groupnum: 1].Value;
        if (!string.IsNullOrWhiteSpace(nodeType))
        {
            value = value.Replace(nodeType, string.Empty);
        }

        var attributeName = GetAttributeNameRegex().Match(xPath).Groups[groupnum: 1].Value;
        if (nodeType.Contains("a:") && !string.IsNullOrWhiteSpace(attributeName))
        {
            foreach (var element in elements)
            {
                element.Attribute(attributeName)?.SetValue(value);
            }
        }
        else
        {
            foreach (var element in elements)
            {
                element.SetValue(value);
            }
        }
    }

    /// <summary>
    /// This method contains logic to rearrange elements in the SOAP document based on provided settings.
    /// </summary>
    /// <param name="document">The document to rearrange.</param>
    /// <param name="settings">Parsing settings.</param>
    /// <returns>The transformed document.</returns>
    private static XDocument ShuffleIfNeeded(this XDocument document, SoapParserSettings settings)
    {
        if (settings?.ElementsToShuffle == null)
        {
            return document;
        }

        var totalShuffleAttempts = settings.TotalShuffleAttempts;
        var namespaceManager = document.GetXmlNamespaceManager();
        foreach (var xPath in settings.ElementsToShuffle)
        {
            var elements = document.XPathSelectElements(xPath, namespaceManager);
            foreach (var element in elements)
            {
                var shuffledElement = element.ShuffleUntilOrderIsDifferent(totalShuffleAttempts);
                element.ReplaceWith(shuffledElement);
            }
        }

        return document;
    }

    /// <summary>
    /// Shuffle the provided element until the order is different.
    /// </summary>
    /// <param name="element">The element to shuffle.</param>
    /// <param name="totalAttempts">To avoid staying in an endless loop</param>
    /// <returns>A new <see cref="XElement"/> with it's child elements shuffled.</returns>
    private static XElement ShuffleUntilOrderIsDifferent(this XElement element, int totalAttempts)
    {
        List<XElement> previousOrder = null;
        var attempts = 0;
        totalAttempts = totalAttempts < 1
            ? SoapParserSettings.DefaultTotalShuffleAttemptsLowerBounds
            : Math.Min(totalAttempts, SoapParserSettings.TotalShuffleAttemptsUpperBounds);

        while (true)
        {
            var elementsList = element.Elements().ToList();
            elementsList = [.. elementsList.OrderBy(_ => Random.Shared.Next())];

            if ((previousOrder != null && elementsList.SequenceEqual(previousOrder)) || attempts >= totalAttempts)
            {
                previousOrder = new List<XElement>(elementsList);
                break;
            }

            previousOrder ??= new List<XElement>(elementsList);
            attempts++;
        }

        return new XElement(element.Name, previousOrder, element.Attributes());
    }

    /// <summary>
    /// Add additional elements to the document.
    /// </summary>
    /// <param name="document">The document to add additional elements to.</param>
    /// <param name="settings">Parsing settings.</param>
    /// <returns>The transformed document.</returns>
    private static XDocument AddAdditionalElementsIfNeeded(this XDocument document, SoapParserSettings settings)
    {
        if (settings?.AdditionalElementsToAdd == null)
        {
            return document;
        }

        var namespaceManager = document.GetXmlNamespaceManager();
        foreach (var (xPath, value) in settings.AdditionalElementsToAdd)
        {
            var elements = document.XPathSelectElements(xPath, namespaceManager);
            foreach (var element in elements)
            {
                foreach (var additionalElement in value.Select(kvp => new XElement(kvp.Key, kvp.Value)))
                {
                    element.Add(additionalElement);
                }
            }
        }

        return document;
    }

    /// <summary>
    /// Add additional attributes to the document.
    /// </summary>
    /// <param name="document">The document to add additional attributes to.</param>
    /// <param name="settings">Parsing settings.</param>
    /// <returns>The transformed document.</returns>
    private static XDocument AddAdditionalAttributesIfNeeded(this XDocument document, SoapParserSettings settings)
    {
        if (settings?.AdditionalAttributesToAdd == null)
        {
            return document;
        }

        var namespaceManager = document.GetXmlNamespaceManager();
        foreach (var (xPath, value) in settings.AdditionalAttributesToAdd)
        {
            var elements = document.XPathSelectElements(xPath, namespaceManager);
            foreach (var element in elements)
            {
                foreach (var attribute in value.Select(kvp => new XAttribute(kvp.Key, kvp.Value)))
                {
                    element.SetAttributeValue(attribute.Name, attribute.Value);
                }
            }
        }

        return document;
    }

    /// <summary>
    /// Regex for identifying node type.
    /// </summary>
    /// <returns>The <see cref="Regex"/>.</returns>
    [GeneratedRegex("^((a|e)\\:)")]
    private static partial Regex GetNodeTypeRegex();

    /// <summary>
    /// Regex for identifying attribute name.
    /// </summary>
    /// <returns>The <see cref="Regex"/>.</returns>
    [GeneratedRegex("\\[\\@(.*)\\=")]
    private static partial Regex GetAttributeNameRegex();
}