using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// The sample container, containing the request and response.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="Sample"/>/
/// </remarks>
/// <param name="request">
/// The <see cref="RequestSample"/> related to the call.
/// </param>
/// <param name="responseSample">
/// The <see cref="ResponseSample"/> related to the call.
/// </param>
public class Sample(RequestSample request, ResponseSample responseSample)
{

    /// <summary>
    /// The <see cref="RequestSample"/> related to the call.
    /// </summary>
    public RequestSample Request { get; set; } = request;

    /// <summary>
    /// The <see cref="ResponseSample"/> related to the call.
    /// </summary>
    public ResponseSample Response { get; set; } = responseSample;

    /// <summary>
    /// Save this sample to disk at the given <paramref name="baseDirectory"/>.
    /// </summary>
    /// <param name="baseDirectory">
    /// The base directory where it stores all the rawsamples.
    /// </param>
    /// <param name="logger">
    /// For logging exceptions.
    /// </param>
    public void Save(string baseDirectory, ILogger logger)
    {
        try
        {
            var now = DateTime.UtcNow;
            var date = $"{now.Year}-{now.Month}-{now.Day}";
            var dateDirectory = Path.Combine(baseDirectory, date);

            if (!Directory.Exists(dateDirectory))
            {
                _ = Directory.CreateDirectory(dateDirectory);
            }

            var path = this.Request.Path[1..];
            path = path.Replace('/', '_');

            var fileName = $"{path}.txt";
            var filePath = Path.Combine(dateDirectory, fileName);

            var data = JsonConvert.SerializeObject(this, Formatting.Indented);

            using var write = File.AppendText(filePath);
            write.WriteLine(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not save the sample");
        }
    }
}
