using System;
using System.Collections.Concurrent;
using System.Linq;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="INatsConnection"/>.
/// </summary>
public sealed class NatsConnection : INatsConnection
{
    /// <summary>
    /// The maximum of retries when trying to subscribe to a specific subject.
    /// </summary>
    public const int SubscribeMaxRetries = 10;

    private readonly ConcurrentDictionary<IConnection, ConcurrentDictionary<Guid, ConcurrentBag<IAsyncSubscription>>>
        connections = new();
    private readonly ILogger<NatsConnection> logger;
    private readonly INatsConnectionFactory connectionFactory;
    private readonly INatsConnectionOptions connectionOptions;
    private readonly INatsServerOptionsManager natsServerOptionsManager;
    private IConnection currentConnection;
    private bool isDisposed;

    /// <summary>
    /// EventHandler that is invoked when the connection is changed.
    /// </summary>
    public event EventHandler<EventArgs> OnConnectionChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="NatsConnection" /> class.
    /// </summary>
    /// <param name="logger">Instance of <see cref="ILogger{NatsConfiguration}"/>.</param>
    /// <param name="connectionFactory">Instance of <see cref="INatsConnectionFactory"/>.</param>
    /// <param name="connectionOptions">Instance of <see cref="INatsConnectionOptions"/>.</param>
    /// <param name="natsServerOptionsManager">Instance of <see cref="INatsServerOptionsManager"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Is thrown when either of <paramref name="logger"/>, <paramref name="connectionFactory"/>,
    /// <paramref name="connectionOptions"/> or <paramref name="natsServerOptionsManager"/> are null.
    /// </exception>
    public NatsConnection(ILogger<NatsConnection> logger, INatsConnectionFactory connectionFactory,
        INatsConnectionOptions connectionOptions, INatsServerOptionsManager natsServerOptionsManager)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        this.connectionOptions = connectionOptions ?? throw new ArgumentNullException(nameof(connectionOptions));
        this.natsServerOptionsManager = natsServerOptionsManager ?? throw new ArgumentNullException(nameof(natsServerOptionsManager));

        this.connectionOptions.OnError += this.OnError;
        this.connectionOptions.OnClosed += this.OnClosed;
        this.connectionOptions.OnServerDiscovered += this.OnServerDiscovered;
        this.connectionOptions.OnDisconnected += this.OnDisconnected;
        this.connectionOptions.OnReconnected += this.OnReconnected;
        this.natsServerOptionsManager.OptionsChanged += this.OnOptionsChanged;
    }

    /// <summary>
    /// Is there a successful connection established.
    /// </summary>
    public bool IsConnected => this.currentConnection?.State == ConnState.CONNECTED;

    /// <summary>
    /// Disposes the connections, timer, onChangeToken and the events.
    /// </summary>
    public void Dispose() => this.Dispose(disposing: true);

    /// <summary>
    /// Tries to publish data to the given subject.
    /// </summary>
    /// <param name="subject">The subject to publish the data to.</param>
    /// <param name="data">The data to publish.</param>
    /// <returns>
    /// Returns true when the data is successfully published on the given subject, otherwise false is returned.
    /// </returns>
    public bool TryPublish(string subject, byte[] data)
    {
        if (!this.IsConnected)
        {
            return false;
        }

        try
        {
            this.currentConnection.Publish(subject, data);
            return true;
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Can't publish the data on subject {Subject}!", subject);
            return false;
        }
    }

    /// <summary>
    /// Tries to subscribe to a specific subject. Expresses interest in the given subject to the NATS Server,
    /// and begins delivering messages to the given event handler.
    /// </summary>
    /// <param name="subject">
    /// The subject on which to listen for messages. The subject can have wildcards (partial: *, full: >).
    /// </param>
    /// <param name="eventHandler">
    /// The <see cref="EventHandler{MsgHandlerEventArgs}"/> invoked when messages are received on the
    /// returned <see cref="IAsyncSubscription"/>.
    /// </param>
    /// <param name="receiverId">The unique identifier of the receiver trying to subscribe.</param>
    public void TrySubscribe(string subject, EventHandler<MsgHandlerEventArgs> eventHandler, Guid receiverId)
    {
        var retryCount = 0;
        while (true)
        {
            if (!this.HasValidSubscribeState(retryCount, subject))
            {
                return;
            }

            try
            {
                this.logger.OnlyInDebug(l =>
                    l.LogDebug("Try to subscribe to subject {Subject}", subject));
                var receiverSubscriptions = this.connections.GetOrAdd(this.currentConnection,
                    new ConcurrentDictionary<Guid, ConcurrentBag<IAsyncSubscription>>());
                var subscriptions = receiverSubscriptions.GetOrAdd(receiverId, []);
                if (subscriptions.Any(subscription => subscription.Subject == subject))
                {
                    this.logger.LogInformation("The receiver with id {Id} is already subscribed to subject " +
                                               "{Subject} for the current connection!", receiverId, subject);
                    return;
                }

                var subscription = this.currentConnection.SubscribeAsync(subject, eventHandler);
                if (subscription == null)
                {
                    continue;
                }

                subscriptions.Add(subscription);
                this.logger.OnlyInDebug(l => l.LogDebug("Successfully subscribed to subject {Subject}", subject));
                return;
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Can't subscribe to subject {Subject}", subject);
                retryCount++;
            }
        }
    }

    /// <summary>
    /// Is the current state valid to allow to subscribe.
    /// </summary>
    /// <param name="retryCount">The current retry count.</param>
    /// <param name="subject">The subject to subscribe to.</param>
    /// <returns><see langword="true"/> if the state is valid; otherwise <see langword="false"/>.</returns>
    private bool HasValidSubscribeState(int retryCount, string subject)
    {
        if (retryCount > SubscribeMaxRetries)
        {
            this.logger.LogError("Can't subscribe to the {Subject} subject, won't retry due to retries " +
                                 "of {SubscribeMaxRetries} exceeded!", subject, SubscribeMaxRetries);
            return false;
        }

        if (this.IsConnected)
        {
            return true;
        }

        this.logger.LogWarning("Can't subscribe to the {Subject} subject as we are not " +
                               "yet connected to the server!", subject);
        return false;
    }

    /// <summary>
    /// Unsubscribe all subjects that are registered for the receiver with the passed <paramref name="receiverId"/>.
    /// </summary>
    /// <param name="receiverId">The unique identifier of the receiver.</param>
    public void Unsubscribe(Guid receiverId)
    {
        foreach (var connection in this.connections)
        {
            if (!connection.Value.TryRemove(receiverId, out var receiverSubscriptions))
            {
                continue;
            }

            foreach (var subscription in receiverSubscriptions)
            {
                subscription.Dispose();
            }
        }
    }

    /// <summary>
    /// Create a new connection using the values from <see cref="NatsOptions"/>.
    /// </summary>
    private void CreateConnection()
    {
        if (this.isDisposed)
        {
            return;
        }

        try
        {
            if (this.currentConnection != null)
            {
                // Either a new connection is created or the current connection will be disposed due to an error,
                // in both cases we do not want to reconnect.
                this.currentConnection.Opts.AllowReconnect = false;
            }
            var tempConnection = this.connectionFactory.CreateConnection(this.connectionOptions);
            if (tempConnection == null)
            {
                this.currentConnection?.Dispose();
                this.currentConnection = null;
                return;
            }

            this.DrainOrCloseAndSetCurrentConnection(tempConnection);
            if (this.connections.TryAdd(this.currentConnection,
                    new ConcurrentDictionary<Guid, ConcurrentBag<IAsyncSubscription>>()))
            {
                this.OnConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (NATSConnectionException e)
        {
            this.logger.LogInformation("Can't create a connection: {Reason}", e.Message);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Can't create a connection");
            this.currentConnection?.Dispose();
            this.currentConnection = null;
        }
    }

    /// <summary>
    /// Drain or Close the <see cref="currentConnection"/> if not null and set it's value
    /// with the passed <see cref="tempConnection"/>.
    /// </summary>
    /// <param name="tempConnection">
    /// The temporary connection that will replace the <see cref="currentConnection"/>.
    /// </param>
    private void DrainOrCloseAndSetCurrentConnection(IConnection tempConnection)
    {
        try
        {
            var state = this.currentConnection?.State;
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (state == ConnState.RECONNECTING)
            {
                // Close the current connection instead of draining it, to avoid throwing a Flush error.
                this.currentConnection?.Close();
            }
            else if (state == ConnState.CONNECTED)
            {
                this.currentConnection?.Drain();
            }

            this.currentConnection = tempConnection;
        }
        catch (Exception e)
        {
            tempConnection.Dispose();
            this.logger.LogError(e, "Can't drain the current connection");
        }
    }

    /// <summary>
    /// Disposes the connection, timer, onChangeToken and the events.
    /// </summary>
    /// <param name="disposing">Whether this should dispose or not.</param>
    private void Dispose(bool disposing)
    {
        if (!disposing || this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.natsServerOptionsManager.OptionsChanged -= this.OnOptionsChanged;
        this.connectionOptions.OnError -= this.OnError;
        this.connectionOptions.OnClosed -= this.OnClosed;
        this.connectionOptions.OnServerDiscovered -= this.OnServerDiscovered;
        this.connectionOptions.OnDisconnected -= this.OnDisconnected;
        this.connectionOptions.OnReconnected -= this.OnReconnected;
        this.currentConnection?.Dispose();
        this.currentConnection = null;
    }

    /// <summary>
    /// Callback when the options is changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="eventArgs">Instance of <see cref="NatsOptionsEventArgs"/>.</param>
    private void OnOptionsChanged(object sender, NatsOptionsEventArgs eventArgs)
    {
        this.logger.OnlyInDebug(log =>
            log.LogDebug("The NatsOptions changed {ConnectionDescription} new connection is needed,",
                eventArgs.IsNewConnectionNeeded ? "and a" : "but no"));

        if (this.IsConnected && !eventArgs.IsNewConnectionNeeded)
        {
            return;
        }
        this.CreateConnection();
    }

    /// <summary>
    /// Event callback when an error is encountered.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="ErrEventArgs"/> args containing the error information.</param>
    private void OnError(object sender, ErrEventArgs args)
        => this.logger.LogError("Server: {Connection} - Error: {Error} - Subject: {Subject}",
            args.Conn, args.Error, args.Subscription.Subject);

    /// <summary>
    /// Event callback when a server is discovered.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="ConnEventArgs"/> args containing information about the connection.</param>
    private void OnServerDiscovered(object sender, ConnEventArgs args)
        => this.logger.LogInformation(args.Error, "A new server has joined the cluster: {DiscoveredServers}",
            string.Join(", ", args.Conn.DiscoveredServers));

    /// <summary>
    /// Event callback when a connection is disconnected.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="ConnEventArgs"/> args containing information about the connection.</param>
    private void OnDisconnected(object sender, ConnEventArgs args)
        => this.logger.LogWarning(args.Error, "Connection Disconnected: {Connection}", args.Conn);

    /// <summary>
    /// Event callback when a connection is reconnected.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="ConnEventArgs"/> args containing information about the connection.</param>
    private void OnReconnected(object sender, ConnEventArgs args)
        => this.logger.LogInformation(args.Error, "Connection Reconnected: {Connection}", args.Conn);

    /// <summary>
    /// Event callback when a connection is closed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="ConnEventArgs"/> args containing information about the connection.</param>
    private void OnClosed(object sender, ConnEventArgs args)
    {
        this.logger.LogInformation(args.Error, "Connection Closed: {Connection}", args.Conn);
        if (this.connections.TryRemove(args.Conn, out var receiverSubscriptions))
        {
            foreach (var receiverSubscription in receiverSubscriptions
                         .Select(receiverSubscription => receiverSubscription.Value))
            {
                this.DisposeSubscriptionsAndClearConcurrentBag(receiverSubscription);
            }
        }
        args.Conn.Dispose();
    }

    /// <summary>
    /// Disposes all the <see cref="IAsyncSubscription"/> inside the passed <paramref name="receiverSubscription"/>.
    /// </summary>
    /// <param name="receiverSubscription">The <see cref="ConcurrentBag{T}"/> instance.</param>
    private void DisposeSubscriptionsAndClearConcurrentBag(ConcurrentBag<IAsyncSubscription> receiverSubscription)
    {
        foreach (var subscription in receiverSubscription)
        {
            try
            {
                subscription.Dispose();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Can't dispose the IAsyncSubscription");
            }
        }

        try
        {
            receiverSubscription.Clear();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Can't clear the ConcurrentBag");
        }
    }
}