using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Schema;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;

namespace TestsSupport.Soap;

/// <summary>
/// Validates a SOAP message against the schema.
/// </summary>
public static class SoapValidator
{
    private static readonly Dictionary<Assembly, XmlSchemaResolver> SchemaResolvers = [];
    private static readonly Dictionary<Type, XmlSchemaSet> SchemaSets = [];

    /// <summary>
    /// Validates the provided SOAP encoded message against the schema.
    /// </summary>
    /// <param name="soap">The SOAP encoded message to validate.</param>
    /// <param name="type">The type to get the expected validation errors for.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that contains a list of <see cref="IValidationError"/> that contains the validation errors.
    /// </returns>
    public static async Task<IList<IValidationError>> GetExpectedValidationErrorsAsync(string soap, Type type)
    {
        var assembly = type.Assembly;
        if (!SchemaResolvers.TryGetValue(assembly, out var schemaResolver))
        {
            schemaResolver = new XmlSchemaResolver(assembly);
            SchemaResolvers.Add(assembly, schemaResolver);
        }

        if (!SchemaSets.TryGetValue(type, out var schemaSet))
        {
            schemaSet = GetSchemaSet(type, schemaResolver);
            SchemaSets.Add(type, schemaSet);
        }

        var validateWithStream = new ValidateXmlWithStream(schemaSet);
        using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(soap);
        await writer.FlushAsync();
        stream.Position = 0;

        _ = validateWithStream.Validate(stream, out var errors);
        return errors;
    }

    /// <summary>
    /// Gets the schema set for the provided type.
    /// </summary>
    /// <param name="type">The type to get the schema set for.</param>
    /// <param name="schemaResolver">
    /// The <see cref="XmlSchemaResolver"/> to use to get the schema.
    /// </param>
    /// <returns>The <see cref="XmlSchemaSet"/> for the provided type.</returns>
    /// <exception cref="ArgumentException">
    /// Throws an <see cref="ArgumentException"/> if no schema definitions are found for the provided type.
    /// </exception>
    private static XmlSchemaSet GetSchemaSet(Type type, XmlSchemaResolver schemaResolver)
    {
        var schemaSet = new XmlSchemaSet();
        var schemaDefinitions = type.GetCustomAttributes<SchemaDefinitionAttribute>();
        var schemaDefinitionAttributes = schemaDefinitions as SchemaDefinitionAttribute[] ?? schemaDefinitions.ToArray();
        if (schemaDefinitions == null || !schemaDefinitionAttributes.Any())
        {
            throw new ArgumentException($"No schema definitions found for type {type.FullName}");
        }

        foreach (var schemaDefinition in schemaDefinitionAttributes)
        {
            var schema = schemaResolver.GetSchema(schemaDefinition.SchemaPath, schemaDefinition.ResourceName);
            _ = schemaSet.Add(schema);
        }
        return schemaSet;
    }
}