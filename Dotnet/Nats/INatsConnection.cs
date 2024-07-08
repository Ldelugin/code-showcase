using System;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Defines the configuration for NATS that handles the connection. 
/// </summary>
public interface INatsConnection : IDisposable
{
    /// <summary>
    /// EventHandler that is invoked when the connection is changed.
    /// </summary>
    event EventHandler<EventArgs> OnConnectionChanged;

    /// <summary>
    /// Is there a successful connection established.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Tries to publish data to the given subject.
    /// </summary>
    /// <param name="subject">The subject to publish the data to.</param>
    /// <param name="data">The data to publish.</param>
    /// <returns>
    /// Returns true when the data is successfully published on the given subject, otherwise false is returned.
    /// </returns>
    bool TryPublish(string subject, byte[] data);

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
    void TrySubscribe(string subject, EventHandler<MsgHandlerEventArgs> eventHandler, Guid receiverId);

    /// <summary>
    /// Unsubscribe all subjects that are registered for the receiver with the passed <paramref name="receiverId"/>.
    /// </summary>
    /// <param name="receiverId">The unique identifier of the receiver.</param>
    void Unsubscribe(Guid receiverId);
}