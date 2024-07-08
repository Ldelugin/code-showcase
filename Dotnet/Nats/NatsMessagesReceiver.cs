using System;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="IMessageReceiver"/> to handle receiving messages from a NATS server.
/// </summary>
public sealed class NatsMessagesReceiver : NatsMessagesBase, IMessageReceiver
{
    -- redacted --

    /// <summary>
    /// EventHandler that is invoked each time a log message is received.
    /// </summary>
    public event EventHandler<-- redacted --> LogReceived;

    /// <summary>
    /// EventHandler that is invoked each time a reboot request is received.
    /// </summary>
    public event EventHandler<-- redacted --> RebootRequestReceived;

    -- redacted --

    private readonly Guid receiverIdentifier;

    /// <summary>
    /// Creates a new instance of <see cref="NatsMessagesReceiver"/>.
    /// </summary>
    /// <param name="natsConnection">Instance of <see cref="INatsConnection"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger{NatsMessagesPoster}"/>.</param>
    public NatsMessagesReceiver(INatsConnection natsConnection, ILogger<NatsMessagesReceiver> logger)
        : base(natsConnection, logger)
    {
        this.receiverIdentifier = Guid.NewGuid();
        // In case the connection is already made before this class is created!
        this.OnConnected();
    }

    /// <summary>
    /// Callback that is called when there is a successful connection established.
    /// </summary>
    protected override void OnConnected()
    {
        -- redacted --
        this.NatsConnection.TrySubscribe($"{NatsMessagesSubjects.Log}.>",
            this.OnLogMessageReceived, this.receiverIdentifier);
        this.NatsConnection.TrySubscribe($"{NatsMessagesSubjects.Reboot}.*",
            this.OnRebootRequestReceived, this.receiverIdentifier);
        -- redacted --
    }

    /// <summary>
    /// Disposes the connection, configuration and the events.
    /// </summary>
    /// <param name="disposing">Whether this should dispose or not.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.NatsConnection.Unsubscribe(this.receiverIdentifier);
        }

        base.Dispose(disposing);
    }

    -- redacted --

    /// <summary>
    /// Callback which is invoked each time a log message is received.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The event arguments.</param>
    private void OnLogMessageReceived(object sender, MsgHandlerEventArgs args) =>
        this.Invoke(this.LogReceived, args, nameof(this.LogReceived));

    /// <summary>
    /// Invoke the <see cref="RebootRequestReceived"/> event when the a request for a reboot was received.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="args">The event arguments.</param>
    private void OnRebootRequestReceived(object sender, MsgHandlerEventArgs args) =>
        this.Invoke(this.RebootRequestReceived, args, nameof(this.RebootRequestReceived));

    -- redacted --

    /// <summary>
    /// Invoke the <paramref name="eventHandler"/> with the provided <paramref name="args"/>.
    /// </summary>
    /// <param name="eventHandler">The event to invoke.</param>
    /// <param name="args">The event arguments to get the data and subject from.</param>
    /// <param name="eventName">The name of the event.</param>
    private void Invoke(EventHandler<-- redacted --> eventHandler, MsgHandlerEventArgs args, string eventName)
    {
        try
        {
            eventHandler?.Invoke(this, new -- redacted --
            {
                Data = args.Message.Data,
                Subject = args.Message.Subject
            });
        }
        catch (Exception e)
        {
            this.Logger?.LogError(e, "Cannot invoke the event {Event}", eventName);
        }
    }
}