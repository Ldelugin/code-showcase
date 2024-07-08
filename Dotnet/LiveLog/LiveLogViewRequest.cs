using System;

namespace -- redacted --;

/// <summary>
/// A representation of a request object that is send to retrieve any logs in the LiveLogView.
/// </summary>
public class LiveLogViewRequest
{
    /// <summary>
    /// An array of log levels to search on.
    /// </summary>
    /// <remarks>
    /// Using int instead of the enum value as LiteDb can't properly query on enum values.
    /// </remarks>
    public int[] LogLevels { get; set; }

    /// <summary>
    /// An array of service names to search on.
    /// </summary>
    /// <remarks>
    /// Using int instead of the enum value as LiteDb can't properly query on enum values.
    /// </remarks>
    public int[] ServiceNames { get; set; }

    /// <summary>
    /// An array of process ids to search on.
    /// </summary>
    public int[] ProcessIds { get; set; }

    /// <summary>
    /// The criteria to search for in the message property of the log.
    /// </summary>
    public string SearchInMessage { get; set; }

    /// <summary>
    /// The start of the time bucket.
    /// </summary>
    public TimeSpan? StartBucket { get; set; }

    /// <summary>
    /// The end of the time bucket.
    /// </summary>
    public TimeSpan? EndBucket { get; set; }

    /// <summary>
    /// The maximum number of log entries that should be returned.
    /// </summary>
    public int? Limit { get; set; }
}
