using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace -- redacted --;

/// <summary>
/// Sample container for the request.
/// </summary>
/// <remarks>
/// Creates a new instance for the <see cref="RequestSample"/>.
/// </remarks>
/// <param name="user">
/// The <see cref="ClaimsPrincipal"/> related with the request.
/// </param>
/// <param name="headers">
/// A <see cref="IHeaderDictionary"/> with all the headers, related to the request.
/// </param>
public class RequestSample(ClaimsPrincipal user, IHeaderDictionary headers)
{

    /// <summary>
    /// The <see cref="DateTime"/> in UTC when this request is received.
    /// </summary>
    public DateTime ReceivedUTC { get; set; }

    /// <summary>
    /// The scheme related with this request. 
    /// </summary>
    public string Scheme { get; set; }

    /// <summary>
    /// The host related with this request.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// The relative path to this request.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// The querystring related with this request.
    /// </summary>
    public string QueryString { get; set; }

    /// <summary>
    /// The user related with this request.
    /// </summary>
    public UserSample User { get; set; } = new UserSample(user);

    /// <summary>
    /// The headers related with this request.
    /// </summary>
    public HeadersSample Headers { get; set; } = new HeadersSample(headers);

    /// <summary>
    /// The raw body of this request.
    /// </summary>
    public string Body { get; set; }
}
