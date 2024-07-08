using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Type = System.Type;

namespace TestsSupport.Extensions;

public static class ReflectionExtensions
{
    public static FieldInfo[] GetPrivateFields(this Type type) =>
        type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

    public static FieldInfo GetPrivateFieldWithType(this Type type, Type fieldType) =>
        type.GetPrivateFields().FirstOrDefault(field => field.FieldType == fieldType ||
                                                        field.FieldType.GetInterfaces().Contains(fieldType) ||
                                                        field.FieldType.IsAssignableFrom(fieldType));

    public static FieldInfo GetPrivateFieldWithName(this Type type, string name) =>
        type.GetPrivateFields().FirstOrDefault(field => field.Name == name);

    public static bool HasDefaultConstructor(this Type type)
        => type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;

    public static MemberInfo GetMemberInfoFromSelector<TOptions, TProp>(this Expression<Func<TOptions, TProp>> selector)
    {
        MemberExpression memberExpression;
        if (selector.Body is UnaryExpression unaryExpression)
        {
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
        {
            memberExpression = (MemberExpression)selector.Body;
        }

        return memberExpression.Member;
    }

    public static string GetFullQualified-- redacted --OptionsKey(this MemberInfo info)
        => $"{-- redacted --}:{info.GetKeyFromMemberInfo()}";

    public static string GetKeyFromMemberInfo(this MemberInfo info)
    {
        var builder = new StringBuilder();
        var baseType = info.DeclaringType?.BaseType;
        if (baseType != null && baseType != typeof(object))
        {
            _ = builder.Append(baseType.Name);
            _ = builder.Append(value: ':');
        }

        _ = builder.Append(info.DeclaringType?.Name);
        _ = builder.Append(value: ':');
        _ = builder.Append(info.Name);
        return builder.ToString();
    }

    private static Func<PropertyInfo, string[], bool> IsNestedProperty => (info, sectionNames) =>
        info.IsDefined(typeof(IsNestedAttribute), inherit: false) && sectionNames.Contains(info.Name);

    public static void Set-- redacted --OptionsValues(this -- redacted -- -- redacted --, string key, string value)
    {
        var sections = key.Split(':');
        var propertyName = sections.LastOrDefault();
        var optionsName = sections.LastOrDefault(section => section != propertyName);

        SetValue(GetOptionsInstance(sections, optionsName, -- redacted --, IsNestedProperty), propertyName, value);

        var namedOptionName = sections.LastOrDefault(section => section != propertyName && section != optionsName);
        SetValue(GetOptionsInstance(sections, namedOptionName, -- redacted --, IsNestedProperty), propertyName, value);
    }

    public static void SetValue(object instance, string propertyName, string value)
    {
        if (instance == null)
        {
            return;
        }

        var property = instance.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
        if (property == null)
        {
            return;
        }

        if (!TryConvertValue(property.PropertyType, value, out var convertedValue, out var error))
        {
            throw new InvalidOperationException("", error);
        }

        property.SetValue(instance, convertedValue);
    }

    public static object GetOptionsInstance(this string[] sectionNames, string sectionName, object instance,
        Func<PropertyInfo, string[], bool> nestedPropertyTypeSelector)
    {
        var optionsName = sectionNames.FirstOrDefault(s => s == sectionName) ?? sectionNames.FirstOrDefault();
        var properties = instance.GetType().GetProperties();

        var property = properties.FirstOrDefault(p => p.Name == optionsName);
        if (property != null)
        {
            return property.GetValue(instance);
        }

        property = properties.FirstOrDefault(p => nestedPropertyTypeSelector(p, sectionNames));
        return property != null
            ? GetOptionsInstance(sectionNames, sectionName, property.GetValue(instance), nestedPropertyTypeSelector)
            : null;
    }

    public static bool TryConvertValue(this Type type, string value, out object result, out Exception error)
    {
        error = null;
        result = null;
        if (type == typeof(object))
        {
            result = value;
            return true;
        }

        if (type.IsNullableType())
        {
            return string.IsNullOrEmpty(value) ||
                   TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
        }

        var converter = TypeDescriptor.GetConverter(type);
        if (!converter.CanConvertFrom(typeof(string)))
        {
            error = new InvalidOperationException($"Type {type} cannot convert from a string.");
            return false;
        }

        try
        {
            result = converter.ConvertFromInvariantString(value);
        }
        catch (Exception e)
        {
            error = new InvalidOperationException(e.Message, e);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the specified argument is null or its value is the default value for its type.
    /// </summary>
    /// <param name="argument">The argument to check.</param>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <returns>
    /// <c>true</c> if the argument is null or its value is the default value for its type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrDefault<T>(this T argument)
    {
        // deal with normal scenarios
        if (argument == null)
        {
            return true;
        }

        // deal with non-null nullables
        var methodType = typeof(T);
        if (Nullable.GetUnderlyingType(methodType) != null)
        {
            return false;
        }

        // deal with boxed value types
        var argumentType = argument.GetType();
        if (!argumentType.IsValueType || argumentType == methodType)
        {
            return false;
        }

        var obj = Activator.CreateInstance(argumentType);
        return obj != null && obj.Equals(argument);
    }
}