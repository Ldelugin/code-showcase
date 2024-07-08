using System;
using -- redacted --;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NATS.Client;

namespace -- redacted --;

public abstract class NatsMessagesTestsBase<T, TNatsMessages> where TNatsMessages : NatsMessagesBase
{
    protected readonly Mock<INatsConnection> NatsConnectionMock = new();
    protected readonly Mock<ILogger<T>> LoggerMock = new();
    protected readonly Mock<IConnection> ConnectionMock = new();
    protected readonly TNatsMessages NatsMessagesBase;

    // ReSharper disable once VirtualMemberCallInConstructor
    protected NatsMessagesTestsBase()
    {
        this.NatsMessagesBase = this.Create();
    }

    protected abstract string Subject { get; }
    protected abstract TNatsMessages Create();

    protected void RaiseOnConnectionChanged()
        => this.NatsConnectionMock.Raise(m => m.OnConnectionChanged += null,
            EventArgs.Empty);

    [TestMethod]
    public void DisposeCleanupTheCorrectMembers()
    {
        // Arrange
        this.RaiseOnConnectionChanged();

        // Act
        this.NatsMessagesBase.Dispose();

        // Assert
        this.NatsConnectionMock.VerifyRemove(m =>
            m.OnConnectionChanged -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
    }
}