using System.Linq;
using Microsoft.AspNetCore.Http;

namespace -- redacted --;

/// <summary>
/// Sample container for headers.
/// </summary>
/// <remarks>
/// Creates new instance of <see cref="HeadersSample"/>.
/// </remarks>
/// <param name="headers">
/// A <see cref="IHeaderDictionary"/> containing all the headers.
/// </param>
public class HeadersSample(IHeaderDictionary headers)
{

    /// <summary>
    /// An array of all the headers.
    /// </summary>
    public string[] Headers { get; set; } = headers.Select(h => $"{h.Key} : {h.Value}").ToArray();
}
