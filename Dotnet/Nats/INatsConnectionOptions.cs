using System;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Defines the nats connection options, <seealso cref="Options"/>.
/// </summary>
public interface INatsConnectionOptions
{
    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection is closed.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnClosed;

    /// <summary>
    /// Represents the method that will handle an event raised
    /// whenever a new server has joined the cluster.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnServerDiscovered;

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection has been disconnected from a server.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnDisconnected;

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection has reconnected to a server.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnReconnected;

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when an error occurs out of band.
    /// </summary>
    public event EventHandler<ErrEventArgs> OnError;

    /// <summary>
    /// The instance of <see cref="Options"/>.
    /// </summary>
    Options InternalNatsOptions { get; }

    /// <summary>
    /// The port of the Nats server.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// The user to authenticate with the Nats server.
    /// </summary>
    string User { get; set; }

    /// <summary>
    /// The password of the user to authenticate with the Nats server.
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Gets or sets the array of servers that the NATS client will connect to.
    /// </summary>
    string[] Servers { get; set; }

    /// <summary>
    /// Bool that indicates whether debug messages should be logged.
    /// </summary>
    bool LogDebug { get; set; }

    /// <summary>
    /// Bool that indicates whether trace message should be logged.
    /// </summary>
    bool LogTrace { get; set; }
}