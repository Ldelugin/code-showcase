using System;
using Microsoft.AspNetCore.Http;

namespace -- redacted --;

/// <summary>
/// Sample container for the response.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ResponseSample"/>.
/// </remarks>
/// <param name="headers">
/// A <see cref="IHeaderDictionary"/> with all the headers, related to the response. 
/// </param>
public class ResponseSample(IHeaderDictionary headers)
{

    /// <summary>
    /// The <see cref="DateTime"/> in UTC when this response is received.
    /// </summary>
    public DateTime ReceivedUTC { get; set; }

    /// <summary>
    /// The status code returned with the response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// The headers related with this response.
    /// </summary>
    public HeadersSample Headers { get; set; } = new HeadersSample(headers);

    /// <summary>
    /// The raw body of this response.
    /// </summary>
    public string Body { get; set; }
}
