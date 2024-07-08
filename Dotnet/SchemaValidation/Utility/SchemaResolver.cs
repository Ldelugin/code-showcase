using System;
using System.IO;

namespace -- redacted --;

public abstract class SchemaResolver
{
    public static bool FileExists(string path) => File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
}
