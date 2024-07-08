using System;
using System.Globalization;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Defines a structure that holds the name of a server and the timestamp that server was alive.
/// </summary>
public class ServerAndTimestamp
{
    public static readonly TimeSpan TrimInterval = TimeSpan.FromSeconds(value: 1);

    /// <summary>
    /// Creates a new instance of <see cref="ServerAndTimestamp"/>.
    /// </summary>
    /// <param name="server">The name of the sever.</param>
    /// <param name="timestamp">The timestamp the server was alive.</param>
    public ServerAndTimestamp(string server, DateTime timestamp)
    {
        this.Server = server;
        this.UpdateTimestamp(timestamp);
    }

    /// <summary>
    /// The name of the server.
    /// </summary>
    public string Server { get; }

    /// <summary>
    /// The timestamp the server was alive.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// The timestamp in a string with the CultureInfo.InvariantCulture applied to it.
    /// </summary>
    public string TimestampAsString => this.Timestamp.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Update the current <see cref="Timestamp"/> and trims the milliseconds.
    /// </summary>
    /// <param name="timestamp">The new timestamp.</param>
    public void UpdateTimestamp(DateTime timestamp) => this.Timestamp = timestamp.Trim(TrimInterval);

    /// <summary>
    /// Creates a new instance of <see cref="ServerAndTimestamp"/> with the given <paramref name="server"/>
    /// and <see cref="DateTime.UtcNow"/> as timestamp.
    /// </summary>
    /// <param name="server">The name of the server.</param>
    /// <returns>
    /// A new instance of <see cref="ServerAndTimestamp"/> with the given <paramref name="server"/>
    /// and <see cref="DateTime.UtcNow"/> as timestamp.
    /// </returns>
    public static ServerAndTimestamp Now(string server) => new(server, DateTime.UtcNow);
}