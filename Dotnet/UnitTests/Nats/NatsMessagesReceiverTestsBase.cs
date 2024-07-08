using System;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NATS.Client;

namespace -- redacted --;

public abstract class NatsMessagesReceiverTestsBase : NatsMessagesTestsBase<NatsMessagesReceiver, NatsMessagesReceiver>
{
    protected override NatsMessagesReceiver Create() => new(this.NatsConnectionMock.Object, this.LoggerMock.Object);

    [TestMethod]
    public void SubscribesOnTheSubjectInTheConstructor()
    {
        // Assert
        this.NatsConnectionMock.Verify(m => m.TrySubscribe(this.Subject,
            It.IsAny<EventHandler<MsgHandlerEventArgs>>(), It.IsAny<Guid>()), Times.Once);
    }

    [TestMethod]
    public void SubscribesOnTheSubject()
    {
        // Arrange
        // This is the call from the constructor.
        this.NatsConnectionMock.Verify(m => m.TrySubscribe(this.Subject,
            It.IsAny<EventHandler<MsgHandlerEventArgs>>(), It.IsAny<Guid>()), Times.Once);

        // Act
        this.RaiseOnConnectionChanged();

        // Assert
        this.NatsConnectionMock.Verify(m => m.TrySubscribe(this.Subject,
            It.IsAny<EventHandler<MsgHandlerEventArgs>>(), It.IsAny<Guid>()), Times.Exactly(callCount: 2));
    }

    [TestMethod]
    public void UnsubscribeWhenDisposed()
    {
        // Arrange
        var receiverId = default(Guid);
        _ = this.NatsConnectionMock.Setup(m => m.TrySubscribe(this.Subject,
                It.IsAny<EventHandler<MsgHandlerEventArgs>>(), It.IsAny<Guid>()))
            .Callback<string, EventHandler<MsgHandlerEventArgs>, Guid>((_, _, id) =>
            {
                receiverId = id;
            });
        this.RaiseOnConnectionChanged();

        // Act
        this.NatsMessagesBase.Dispose();

        // Assert
        this.NatsConnectionMock.Verify(m => m.Unsubscribe(receiverId), Times.Once);
    }
}