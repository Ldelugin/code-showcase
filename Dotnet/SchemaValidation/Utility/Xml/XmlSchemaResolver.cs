using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace -- redacted --;

public class XmlSchemaResolver(Assembly assembly) : SchemaResolver
{
    private readonly Assembly assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

    public XmlSchema GetSchema(string schemaPath, string resourceName, Dictionary<string, string> overrides = null)
    {
        XmlSchema schema;

        if (String.IsNullOrEmpty(schemaPath) || !FileExists(schemaPath))
        {
            schema = this.LoadXmlSchemaFromResource(resourceName, overrides);

            if (schema == null)
            {
                throw new ArgumentException($"Could not load schema as a resource {resourceName}");
            }

            return schema;
        }

        schema = LoadXmlSchemaFromFile(schemaPath, overrides);

        if (schema == null)
        {
            throw new ArgumentException($"Could not load schema from file {schemaPath}");
        }

        return schema;
    }

    protected static XmlSchema LoadXmlSchemaFromFile(string path, Dictionary<string, string> overrides = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path), @"path can't be null or empty");
        }

        var data = File.ReadAllText(path);

        if (overrides != null)
        {
            data = data.Replace(overrides);
        }

        return XmlSchema.Read(new XmlTextReader(new StringReader(data)), null);
    }

    protected XmlSchema LoadXmlSchemaFromResource(string resourceName, Dictionary<string, string> overrides = null)
    {
        if (string.IsNullOrEmpty(resourceName))
        {
            throw new ArgumentNullException(nameof(resourceName), @"resourceName can't be null or empty");
        }

        var resources = this.assembly.GetManifestResourceNames();
        var resource = resources.Single(r => r.Contains(resourceName));
        var stream = this.assembly.GetManifestResourceStream(resource);

        if (stream == null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var data = reader.ReadToEnd();

        if (overrides != null)
        {
            data = data.Replace(overrides);
        }

        return XmlSchema.Read(new XmlTextReader(new StringReader(data)), null);
    }
}
