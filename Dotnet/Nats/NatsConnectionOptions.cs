using System;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="INatsConnectionOptions"/> which acts as a wrapper
/// around <see cref="Options"/> to support unit testing.
/// </summary>
public class NatsConnectionOptions : INatsConnectionOptions
{
    private string password;

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection is closed.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnClosed
    {
        add => this.InternalNatsOptions.ClosedEventHandler += value;
        remove => this.InternalNatsOptions.ClosedEventHandler -= value;
    }

    /// <summary>
    /// Represents the method that will handle an event raised
    /// whenever a new server has joined the cluster.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnServerDiscovered
    {
        add => this.InternalNatsOptions.ServerDiscoveredEventHandler += value;
        remove => this.InternalNatsOptions.ServerDiscoveredEventHandler -= value;
    }

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection has been disconnected from a server.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnDisconnected
    {
        add => this.InternalNatsOptions.DisconnectedEventHandler += value;
        remove => this.InternalNatsOptions.DisconnectedEventHandler -= value;
    }

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when a connection has reconnected to a server.
    /// </summary>
    public event EventHandler<ConnEventArgs> OnReconnected
    {
        add => this.InternalNatsOptions.ReconnectedEventHandler += value;
        remove => this.InternalNatsOptions.ReconnectedEventHandler -= value;
    }

    /// <summary>
    /// Represents the method that will handle an event raised
    /// when an error occurs out of band.
    /// </summary>
    public event EventHandler<ErrEventArgs> OnError
    {
        add => this.InternalNatsOptions.AsyncErrorEventHandler += value;
        remove => this.InternalNatsOptions.AsyncErrorEventHandler -= value;
    }

    /// <summary>
    /// Creates a new instance of <see cref="NatsConnectionOptions"/>.
    /// </summary>
    /// <param name="options">Instance of <see cref="Options"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public NatsConnectionOptions(Options options)
    {
        this.InternalNatsOptions = options ?? throw new ArgumentNullException(nameof(options));
        this.InternalNatsOptions.NoRandomize = true;
    }

    /// <summary>
    /// The instance of <see cref="Options"/>.
    /// </summary>
    public Options InternalNatsOptions { get; }

    /// <summary>
    /// The port of the Nats server.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// The user to authenticate with the Nats server.
    /// </summary>
    public string User
    {
        get => this.InternalNatsOptions.User;
        set => this.InternalNatsOptions.User = value;
    }

    /// <summary>
    /// The password of the user to authenticate with the Nats server.
    /// </summary>
    public string Password
    {
        get => this.password;
        set => this.InternalNatsOptions.Password = this.password = value;
    }

    /// <summary>
    /// Gets or sets the array of servers that the NATS client will connect to.
    /// </summary>
    public string[] Servers
    {
        get => this.InternalNatsOptions.Servers;
        set => this.InternalNatsOptions.Servers = value;
    }

    /// <summary>
    /// Bool that indicates whether debug messages should be logged.
    /// </summary>
    public bool LogDebug { get; set; }

    /// <summary>
    /// Bool that indicates whether trace message should be logged.
    /// </summary>
    public bool LogTrace { get; set; }
}