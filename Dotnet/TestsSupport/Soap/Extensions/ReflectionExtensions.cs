using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestsSupport.Soap.Extensions;

/// <summary>
/// Contains extension methods for reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets the provided type properties that could be filtered by the provided predicate.
    /// </summary>
    /// <param name="type">The type to get the properties from.</param>
    /// <param name="predicate">The optional predicate to test each element against.</param>
    /// <returns>
    /// An array of <see cref="PropertyInfo"/> that contains elements from the input sequence that satisfy the condition.
    /// </returns>
    public static PropertyInfo[] GetProperties(this Type type, Func<PropertyInfo, bool> predicate = null) =>
        predicate == null ? type.GetProperties() : type.GetProperties().Where(predicate).ToArray();

    /// <summary>
    ///  Try to get the <see cref="XNamespace"/> and <see cref="XAttribute"/> from the provided value.
    /// </summary>
    /// <param name="value">
    ///  The value to get the <see cref="XNamespace"/> and <see cref="XAttribute"/> from.
    /// </param>
    /// <param name="ns">
    /// The <see cref="XNamespace"/> from the provided value; otherwise <c>null</c>.
    /// </param>
    /// <param name="xAttribute">
    /// The <see cref="XAttribute"/> from the provided value; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="XNamespace"/> and <see cref="XAttribute"/> is found; otherwise <c>false</c>.
    /// </returns>
    public static bool TryGetXNamespaceAndXAttribute(this object value, out XNamespace ns, out XAttribute xAttribute)
    {
        var xmlTypeAttribute = value?.GetType().GetCustomAttribute<XmlTypeAttribute>();
        ns = xmlTypeAttribute?.Namespace;
        xAttribute = ns?.ConvertToXAttribute();
        return ns != null && xAttribute != null;
    }

    /// <summary>
    /// Try to get the <see cref="XmlArrayAttribute"/> and <see cref="XmlArrayItemAttribute"/> from the provided type.
    /// </summary>
    /// <param name="type">
    /// The type to get the <see cref="XmlArrayAttribute"/> and <see cref="XmlArrayItemAttribute"/> from.
    /// </param>
    /// <param name="xmlArrayAttribute">
    /// The <see cref="XmlArrayAttribute"/> from the provided type; otherwise <c>null</c>.
    /// </param>
    /// <param name="xmlArrayItemAttribute">
    /// The <see cref="XmlArrayItemAttribute"/> from the provided type; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="XmlArrayAttribute"/> or <see cref="XmlArrayItemAttribute"/> is found; otherwise <c>false</c>.
    /// </returns>
    public static bool TryGetXmlArrayAttributes(this Type type, out XmlArrayAttribute xmlArrayAttribute,
        out XmlArrayItemAttribute xmlArrayItemAttribute)
    {
        xmlArrayAttribute = type.GetCustomAttribute<XmlArrayAttribute>();
        xmlArrayItemAttribute = type.GetCustomAttribute<XmlArrayItemAttribute>();
        return xmlArrayAttribute != null || xmlArrayItemAttribute != null;
    }

    /// <summary>
    /// Try to get the <see cref="XmlArrayAttribute"/> and <see cref="XmlArrayItemAttribute"/> from the provided parameter.
    /// </summary>
    /// <param name="parameterInfo">
    /// The parameter to get the <see cref="XmlArrayAttribute"/> and <see cref="XmlArrayItemAttribute"/> from.
    /// </param>
    /// <param name="xmlArrayAttribute">
    /// The <see cref="XmlArrayAttribute"/> from the provided parameter; otherwise <c>null</c>.
    /// </param>
    /// <param name="xmlArrayItemAttribute">
    /// The <see cref="XmlArrayItemAttribute"/> from the provided parameter; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="XmlArrayAttribute"/> or <see cref="XmlArrayItemAttribute"/> is found; otherwise <c>false</c>.
    /// </returns>
    public static bool TryGetXmlArrayAttributes(this ParameterInfo parameterInfo, out XmlArrayAttribute xmlArrayAttribute,
        out XmlArrayItemAttribute xmlArrayItemAttribute)
    {
        xmlArrayAttribute = parameterInfo.GetCustomAttribute<XmlArrayAttribute>();
        xmlArrayItemAttribute = parameterInfo.GetCustomAttribute<XmlArrayItemAttribute>();
        return xmlArrayAttribute != null || xmlArrayItemAttribute != null;
    }

    /// <summary>
    /// Get the generic collection from the provided object.
    /// </summary>
    /// <param name="value">The object to get the generic collection from.</param>
    /// <returns>
    /// Returns the generic collection from the provided object; otherwise <c>null</c>.
    /// </returns>
    public static List<object> GetGenericCollection(this object value) =>
        ((IEnumerable<object>)value)?.ToList();

    /// <summary>
    /// Checks whether the provided type is a generic collection.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    /// <c>true</c> if the provided type is a generic collection; otherwise <c>false</c>
    /// </returns>
    public static bool IsGenericCollection(this Type type)
    {
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
               type.GetInterfaces().Any(i => i.IsGenericCollection());
    }

    /// <summary>
    /// Get the name of the enum value.
    /// </summary>
    /// <remarks>
    /// It could be used to get the name of the enum value when the
    /// enum value is decorated with <see cref="XmlEnumAttribute"/>.
    /// </remarks>
    /// <param name="value"></param>
    /// <returns>The name of the enum value; otherwise the provided value.</returns>
    public static object GetEnumName(this object value)
    {
        if (value == null || !value.GetType().IsEnum)
        {
            return value;
        }

        var enumType = value.GetType();
        var enumName = Enum.GetName(enumType, value);
        if (enumName == null)
        {
            return value;
        }

        var xmlEnumAttribute = enumType.GetField(enumName)?.GetCustomAttribute<XmlEnumAttribute>();
        return xmlEnumAttribute?.Name ?? value;
    }

    /// <summary>
    /// Checks whether the provided value is an XML type.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>
    /// <c>true</c> if the provided value is an XML type; otherwise <c>false</c>.
    /// </returns>
    public static bool IsXmlType(this object value)
    {
        if (value == null)
        {
            return false;
        }

        var type = value.GetType();
        if (!type.IsGenericCollection() || type == typeof(string))
        {
            return value.TryGetXNamespaceAndXAttribute(out _, out _);
        }

        var collection = value.GetGenericCollection();
        if (collection == null || collection.Count == 0)
        {
            return false;
        }

        var firstItem = collection.FirstOrDefault();
        return firstItem?.TryGetXNamespaceAndXAttribute(out _, out _) ?? false;
    }
}