using System.Text;

namespace -- redacted --;

/// <summary>
/// Utility class with string extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Extension method that removes the UTF-8 Preamble from the provided 
    /// <paramref name="input"/> string.
    /// 
    /// The UTF8 Preamble are the 3 bytes sequence at the start of a file 
    /// that indicates it is UTF-8.
    /// </summary>
    /// <param name="input">
    /// The string where the UTF-8 Preamble should be removed.
    /// </param>
    /// <returns>
    /// The input string without UTF-8 Preamble.
    /// </returns>
    public static string RemoveUTF8Preamble(this string input)
    {
        var preamble = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        if (input.StartsWith(preamble))
        {
            return input.Remove(0, preamble.Length);
        }

        return input;
    }
}
