using System;
using System.Collections.Generic;

namespace -- redacted --;

public static class StringExtentions
{
    /// <summary>
    /// Replace all the keys that are present in the string with their corresponding values
    /// </summary>
    /// <param name="str">The string to replace in</param>
    /// <param name="replaces">Each key that is present in the string will bre replaced with the corresponding value</param>
    /// <returns>A new string with all the replaces</returns>
    public static string Replace(this string str, Dictionary<string, string> replaces)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        foreach (var entry in replaces)
        {
            str = str.Replace(entry.Key, entry.Value);
        }

        return str;
    }
}
