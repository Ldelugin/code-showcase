using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using -- redacted --;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Middleware for handling RawSampling, which hooks into the <see cref="HttpContext.Request"/> and 
/// <see cref="HttpContext.Response"/>.
/// </summary>
/// <remarks>
/// Creates new instance of <see cref="RawSamplingMiddleware"/>.
/// </remarks>
/// <param name="next">
/// The next middleware or call in the pipeline.
/// </param>
/// <param name="options">
/// The -- redacted -- options to retrieve the RawSampling options from.
/// </param>
/// <param name="loggerFactory">
/// The <see cref="ILoggerFactory"/> for creating a <see cref="ILogger"/>.
/// </param>
public class RawSamplingMiddleware(RequestDelegate next, IOptionsMonitor<-- redacted --> options, ILoggerFactory loggerFactory)
{
    /// <summary>
    /// The next middleware or call in the pipeline.
    /// </summary>
    private readonly RequestDelegate next = next;

    /// <summary>
    /// The -- redacted -- options to retrieve the RawSampling options from.
    /// </summary>
    private readonly IOptionsMonitor<-- redacted --> options = options ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Logging instance to facilitate logging.
    /// </summary>
    private readonly ILogger logger = loggerFactory.CreateLogger("RawSampling");

    /// <summary>
    /// The invoke method for this middleware.
    /// </summary>
    /// <param name="context">
    /// The given <see cref="HttpContext"/> related to the call.
    /// </param>
    /// <returns>
    /// An async task depending on whether raw sampling is enabled or not.
    /// </returns>
    public async Task Invoke(HttpContext context)
    {
        if (this.options.CurrentValue.RawSamplingOptions.IsEnabled)
        {
            await this.Sample(context);
        }
        else
        {
            await this.next(context);
        }
    }

    /// <summary>
    /// The sample method that retrieves all the given information from the 
    /// <see cref="HttpContext.Request"/> and <see cref="HttpContext.Response"/>.
    /// </summary>
    /// <param name="context">
    /// The given <see cref="HttpContext"/>.
    /// </param>
    /// <returns>
    /// An async task.
    /// </returns>
    private async Task Sample(HttpContext context)
    {
        var request = await FormatRequest(context.Request);

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await this.next(context);

        var response = await FormatResponse(context.Response);

        var sample = new Sample(request, response);
        sample.Save(this.options.CurrentValue.RawSamplingOptions.Path, this.logger);

        await responseBody.CopyToAsync(originalBodyStream);
    }

    /// <summary>
    /// Formats the given <see cref="HttpRequest"/> as an <see cref="RequestSample"/>.
    /// </summary>
    /// <param name="request">
    /// The given <see cref="HttpRequest"/>.
    /// </param>
    /// <returns>
    /// An async task with the formatted <see cref="RequestSample"/>.
    /// </returns>
    private static async Task<RequestSample> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        _ = await request.Body.ReadAsync(buffer);
        _ = request.Body.Seek(offset: 0, SeekOrigin.Begin);

        var bodyAsText = Encoding.UTF8.GetString(buffer);

        var user = request.HttpContext.User;
        var headers = request.Headers;

        var requestSample = new RequestSample(user, headers)
        {
            ReceivedUTC = DateTime.UtcNow,
            Scheme = request.Scheme,
            Host = request.Host.ToString(),
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            Body = bodyAsText
        };

        return requestSample;
    }

    /// <summary>
    /// Formats the given <see cref="HttpResponse"/> as an <see cref="ResponseSample"/>.
    /// </summary>
    /// <param name="response">
    /// The given <see cref="HttpResponse"/>.
    /// </param>
    /// <returns>
    /// An async task with the formatted <see cref="ResponseSample"/>.
    /// </returns>
    private static async Task<ResponseSample> FormatResponse(HttpResponse response)
    {
        _ = response.Body.Seek(offset: 0, SeekOrigin.Begin);

        var text = await new StreamReader(response.Body).ReadToEndAsync();

        _ = response.Body.Seek(offset: 0, SeekOrigin.Begin);

        var headers = response.Headers;

        var responseSample = new ResponseSample(headers)
        {
            ReceivedUTC = DateTime.UtcNow,
            StatusCode = response.StatusCode,
            Body = text
        };

        return responseSample;
    }
}
