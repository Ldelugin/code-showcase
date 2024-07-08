using System;

namespace -- redacted --;

/// <summary>
/// Provides functionality to retrieve logs to show live. 
/// </summary>
public interface ILiveLogService : IDisposable
{
    /// <summary>
    /// Get an array of logs that could be found with the provided <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request criteria.</param>
    /// <returns>
    /// Returns any logs that could be found with the request.
    /// </returns>
    LogModel[] GetLogs(LiveLogViewRequest request);
}
