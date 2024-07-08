using Microsoft.AspNetCore.Builder;

namespace -- redacted --;

/// <summary>
/// Extensions class to setup and use RawSampling. 
/// </summary>
public static class RawSamplingExtensions
{
    /// <summary>
    /// Adds the <see cref="RawSamplingMiddleware"/> as middleware to the provided
    /// <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> that should add the 
    /// <see cref="RawSamplingMiddleware"/>.
    /// </param>
    public static void UseRawSampling(this IApplicationBuilder app) => _ = app.UseMiddleware<RawSamplingMiddleware>();
}
