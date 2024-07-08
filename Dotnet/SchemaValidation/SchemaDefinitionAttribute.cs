using System;

namespace -- redacted --;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SchemaDefinitionAttribute(string schemaPath, string resourceName) : Attribute
{
    public string SchemaPath { get; private set; } = schemaPath;

    public string ResourceName { get; private set; } = resourceName;
}
