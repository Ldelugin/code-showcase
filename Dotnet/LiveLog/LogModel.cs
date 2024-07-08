using System;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Represents a log object that is sent with a request.
/// </summary>
public class LogModel
{
    /// <summary>
    /// The unique identifier of this log object.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The level that indicates the severity of the log.
    /// </summary>
    /// <remarks>
    /// Using int instead of the enum value as LiteDb can't properly query on enum values.
    /// </remarks>
    public byte LogLevel { get; set; }

    /// <summary>
    /// The name of the service that produced the log.
    /// </summary>
    /// <remarks>
    /// Using byte instead of the enum value as LiteDb can't properly query on enum values.
    /// </remarks>
    public byte ServiceName { get; set; }

    /// <summary>
    /// The process identifier of the service that produced the log.
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// The message of the log.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// The timestamp of the log.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Converts given <see cref="Log"/> to a <see cref="LogModel"/>.
    /// </summary>
    /// <param name="log">The <see cref="Log"/> to convert.</param>
    /// <returns>The converted <see cref="LogModel"/>.</returns>
    public static explicit operator LogModel(Log log)
    {
        return new LogModel
        {
            LogLevel = log.LogLevel,
            ServiceName = log.ServiceName,
            Message = log.Message,
            ProcessId = log.ProcessId,
            Timestamp = log.Timestamp
        };
    }

    /// <summary>
    /// Converts given <see cref="LogModel"/> to a <see cref="Log"/>.
    /// </summary>
    /// <param name="logModel">The <see cref="LogModel"/> to convert.</param>
    /// <returns>The converted <see cref="Log"/>.</returns>
    public static explicit operator Log(LogModel logModel)
    {
        return new Log(logModel.Message,
            logModel.Timestamp.ToUniversalTime(),
            logModel.ProcessId,
            logModel.LogLevel,
            logModel.ServiceName);
    }
}
