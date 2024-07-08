using System;
using System.IO;

namespace -- redacted --;

/// <summary>
/// Represents a log object.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="Log"/>.
/// </remarks>
/// <param name="message">The message of the log.</param>
/// <param name="logLevel">The level that indicates the severity of the log.</param>
/// <param name="processId">The process identifier of the service that produced the log.</param>
/// <param name="serviceName">The name of the service that produced the log.</param>
/// <param name="timestamp">The timestamp of the log.</param>
[Serializable]
public struct Log(string message, DateTime timestamp, int processId, byte logLevel, byte serviceName)
{

    /// <summary>
    /// The message of the log.
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// The timestamp of the log.
    /// </summary>
    public DateTime Timestamp { get; set; } = timestamp;

    /// <summary>
    /// The process identifier of the service that produced the log.
    /// </summary>
    public int ProcessId { get; } = processId;

    /// <summary>
    /// The level that indicates the severity of the log.
    /// </summary>
    public byte LogLevel { get; } = logLevel;

    /// <summary>
    /// The name of the service that produced the log.
    /// </summary>
    public byte ServiceName { get; } = serviceName;

    /// <summary>
    /// Converts the <see cref="Log"/> to an byte array.
    /// </summary>
    /// <returns>The converted byte array.</returns>
    public readonly byte[] ToByteArray()
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write(this.Message ?? "");
        writer.Write(this.Timestamp.Ticks);
        writer.Write(this.ProcessId);
        writer.Write(this.LogLevel);
        writer.Write(this.ServiceName);
        return stream.ToArray();
    }

    /// <summary>
    /// Converts the given <paramref name="bytes"/> into a <see cref="Log"/>.
    /// </summary>
    /// <param name="bytes">The byte array.</param>
    /// <returns>The converted <see cref="Log"/>.</returns>
    public static Log FromByteArray(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);
        return new Log(
            reader.ReadString(),
            DateTime.FromBinary(reader.ReadInt64()),
            reader.ReadInt32(),
            reader.ReadByte(),
            reader.ReadByte()
        );
    }
}
