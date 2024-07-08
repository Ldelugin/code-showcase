using System;
using -- redacted --;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// Base class to handle the connection with a NATS server.
/// </summary>
public abstract class NatsMessagesBase : IDisposable
{
    protected readonly ILogger Logger;
    protected readonly INatsConnection NatsConnection;

    /// <summary>
    /// Creates a new instance of <see cref="NatsMessagesBase"/>.
    /// </summary>
    /// <param name="natsConnection">Instance of <see cref="INatsConnection"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger{NatsMessagesPoster}"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Is thrown when <paramref name="natsConnection"/> or when
    /// <paramref name="logger"/> is null.
    /// </exception>
    protected NatsMessagesBase(INatsConnection natsConnection, ILogger logger)
    {
        this.NatsConnection = natsConnection ?? throw new ArgumentNullException(nameof(natsConnection));
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.NatsConnection.OnConnectionChanged += this.OnConnectionChanged;
    }

    /// <summary>
    /// Disposes the connection, configuration and the events.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Callback that is called when there is a successful connection established.
    /// </summary>
    protected virtual void OnConnected()
    {
    }

    /// <summary>
    /// Callback that is invoked from the <see cref="INatsConnection"/> when the connection is changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="eventArgs">The event arguments.</param>
    private void OnConnectionChanged(object sender, EventArgs eventArgs)
    {
        this.Logger.OnlyInDebug(l => l.LogInformation("Nats configuration changed"));
        this.OnConnected();
    }

    /// <summary>
    /// Disposes the connection, configuration and the events.
    /// </summary>
    /// <param name="disposing">Whether this should dispose or not.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        this.NatsConnection.OnConnectionChanged -= this.OnConnectionChanged;
    }
}