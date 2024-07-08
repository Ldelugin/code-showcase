using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NATS.Client;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsConnectionTests
{
    private readonly Mock<ILogger<NatsConnection>> loggerMock = new();
    private readonly Mock<INatsConnectionFactory> connectionFactoryMock = new();
    private readonly Mock<INatsServerOptionsManager> natsServerOptionsManagerMock = new();

    private NatsConnection CreateNatsConnection(IMock<INatsConnectionOptions> natsConnectionOptionsMock)
    {
        return new NatsConnection(this.loggerMock.Object, this.connectionFactoryMock.Object,
            natsConnectionOptionsMock.Object, this.natsServerOptionsManagerMock.Object);
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullException()
        => Assert.That.ConstructorThrowsArgumentNullException<NatsConnection>();

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void DisposeCleanupTheCorrectMembers(bool hasConnection)
    {
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        using (var __ = this.CreateNatsConnection(connectionOptionsMock))
        {
            _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);
            connectionOptionsMock.VerifyAdd(m => m.OnError += It.IsAny<EventHandler<ErrEventArgs>>());
            connectionOptionsMock.VerifyAdd(m => m.OnClosed += It.IsAny<EventHandler<ConnEventArgs>>());
            connectionOptionsMock.VerifyAdd(m => m.OnServerDiscovered += It.IsAny<EventHandler<ConnEventArgs>>());
            connectionOptionsMock.VerifyAdd(m => m.OnDisconnected += It.IsAny<EventHandler<ConnEventArgs>>());
            connectionOptionsMock.VerifyAdd(m => m.OnReconnected += It.IsAny<EventHandler<ConnEventArgs>>());
        }

        if (hasConnection)
        {
            connectionMock.Verify(m => m.Dispose(), Times.Once);
        }

        connectionOptionsMock.VerifyRemove(m => m.OnError -= It.IsAny<EventHandler<ErrEventArgs>>());
        connectionOptionsMock.VerifyRemove(m => m.OnClosed -= It.IsAny<EventHandler<ConnEventArgs>>());
        connectionOptionsMock.VerifyRemove(m => m.OnServerDiscovered -= It.IsAny<EventHandler<ConnEventArgs>>());
        connectionOptionsMock.VerifyRemove(m => m.OnDisconnected -= It.IsAny<EventHandler<ConnEventArgs>>());
        connectionOptionsMock.VerifyRemove(m => m.OnReconnected -= It.IsAny<EventHandler<ConnEventArgs>>());
    }

    [TestMethod]
    public void IsConnectedReturnsFalseIfConnectionIsNull()
    {
        // Arrange
        _ = this.connectionFactoryMock.SetupToReturnNullConnection(out var natsConnectionOptionsMock);

        using var natsConnection = this.CreateNatsConnection(natsConnectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        var isConnected = natsConnection.IsConnected;

        // Assert
        Assert.IsFalse(isConnected);
    }

    [TestMethod]
    [DataRow(ConnState.DISCONNECTED)]
    [DataRow(ConnState.CLOSED)]
    [DataRow(ConnState.RECONNECTING)]
    [DataRow(ConnState.CONNECTING)]
    [DataRow(ConnState.DRAINING_SUBS)]
    [DataRow(ConnState.DRAINING_PUBS)]
    public void IsConnectedReturnsFalseIfConnectionIsNotNullButIsNotInConnectedState(ConnState state)
    {
        // Arrange
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(state);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);

        // Act
        var isConnected = natsConnection.IsConnected;

        // Assert
        Assert.IsFalse(isConnected);
    }

    [TestMethod]
    public void IsConnectedReturnsTrueWhenConnectionIsNotNullAndIsInConnectedState()
    {
        // Arrange
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);

        // Act
        var isConnected = natsConnection.IsConnected;

        // Assert
        Assert.IsTrue(isConnected);
    }

    public static IEnumerable<object[]> GetDataForCreateConnectionCreatesANewConnectionWhenOptionsAreChanged()
    {
        // In case the state is Connected it should call Drain() and not Close().
        yield return new object[]
        {
            ConnState.CONNECTED,
            new Action<Mock<IConnection>>(mock =>
            {
                mock.Verify(m => m.Drain(), Times.Once);
                mock.Verify(m => m.Close(), Times.Never);
            })
        };

        // In case the state is Reconnecting it should call Close() and not Drain().
        yield return new object[]
        {
            ConnState.RECONNECTING,
            new Action<Mock<IConnection>>(mock =>
            {
                mock.Verify(m => m.Drain(), Times.Never);
                mock.Verify(m => m.Close(), Times.Once);
            })
        };

        // In case the state is not Connected or Reconnecting it should not call Drain() and not Close().
        var statesWithoutConnectedAndReconnected = Enum.GetValues<ConnState>()
            .Where(state => state is not ConnState.CONNECTED)
            .Where(state => state is not ConnState.RECONNECTING)
            .ToArray();

        foreach (var state in statesWithoutConnectedAndReconnected)
        {
            yield return new object[]
            {
                state,
                new Action<Mock<IConnection>>(mock =>
                {
                    mock.Verify(m => m.Drain(), Times.Never);
                    mock.Verify(m => m.Close(), Times.Never);
                })
            };
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetDataForCreateConnectionCreatesANewConnectionWhenOptionsAreChanged), DynamicDataSourceType.Method)]
    public void CreateConnectionCreatesANewConnectionWhenOptionsAreChanged(ConnState state, Action<Mock<IConnection>> verifyConnectionMock)
    {
        // Arrange
        var timesConnectionIsCreated = 0;
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(state);
        _ = this.loggerMock
            .Setup(m => m.IsEnabled(LogLevel.Debug))
            .Returns(value: true);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        // Create the initial connection so it's connected.
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);
        timesConnectionIsCreated++;

        // Act
        // Trigger a options change so it's forced to create a new one.
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);
        timesConnectionIsCreated++;

        // Assert
        this.connectionFactoryMock.Verify(m
            => m.CreateConnection(connectionOptionsMock.Object), Times.Exactly(timesConnectionIsCreated));
        this.loggerMock.VerifyLog(LogLevel.Debug, message => message.Contains("changed"),
            Times.Exactly(timesConnectionIsCreated));

        // Verify correct close/drain action (or nothing) happens with the correct state of the connection
        verifyConnectionMock(connectionMock);
    }

    [TestMethod]
    [DataRow(true, 2)]
    [DataRow(false, 1)]
    public void CreateConnectionCreatesANewConnectionWhenIsNewConnectionReturnsTrue(bool isNewConnectionNeeded,
        int timesConnectionIsCreated)
    {
        // Arrange
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        _ = this.loggerMock
            .Setup(m => m.IsEnabled(LogLevel.Debug))
            .Returns(value: true);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        // Create the initial connection so it's connected.
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);

        // Act
        // Trigger a options change so it's forced to create a new one.
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: isNewConnectionNeeded);

        // Assert
        this.connectionFactoryMock.Verify(m
            => m.CreateConnection(connectionOptionsMock.Object), Times.Exactly(timesConnectionIsCreated));
        this.loggerMock.VerifyLog(LogLevel.Debug, message =>
            message.Contains("changed") && message.Contains("new connection is needed"),
            Times.Exactly(callCount: 2));
    }

    [TestMethod]
    public void CreateConnectionDisposesCurrentConnectionWhenExceptionIsThrownOnTheConnectionFactory()
    {
        // Arrange
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);
        this.connectionFactoryMock.Reset();
        _ = this.connectionFactoryMock.SetupToThrow<Exception>(connectionOptionsMock);

        // Act
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);

        // Assert
        this.loggerMock.Verify(LogLevel.Error, Times.Once());
    }

    [TestMethod]
    public void CreateConnectionHandlesExceptionThrownOnDrainCorrectly()
    {
        // Arrange
        const int port = 4223;
        const string password = "ABC123!";
        const string user = "Client";
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = connectionMock.Setup(m => m.Drain()).Throws<Exception>();
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(isNewConnectionNeeded: true);

        // Act
        var natsOptions = new NatsOptions
        {
            NatsServerPort = port,
            NatsServerClientPassword = password,
            NatsServerClientUserName = user
        };
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(natsOptions, isNewConnectionNeeded: true);
        _ = connectionOptionsMock.SetupAsDefault(natsOptions);

        // Assert
        connectionMock.Verify(m => m.Drain(), Times.Once());
    }

    [TestMethod]
    public void CreateConnectionHandlesExceptionThrownOnCloseCorrectly()
    {
        // Arrange
        const int port = 4223;
        const string password = "ABC123!";
        const string user = "Client";
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.RECONNECTING);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = connectionMock.Setup(m => m.Close()).Throws<Exception>();
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        var natsOptions = new NatsOptions
        {
            NatsServerPort = port,
            NatsServerClientPassword = password,
            NatsServerClientUserName = user
        };
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(natsOptions);
        _ = connectionOptionsMock.SetupAsDefault(natsOptions);

        // Assert
        connectionMock.Verify(m => m.Close(), Times.Once());
    }

    [TestMethod]
    public void TrySubscribeReturnsWhenRetriesExceedsSubscribeMaxRetries()
    {
        // Arrange
        const int retries = NatsConnection.SubscribeMaxRetries + 1;
        const string subject = NatsMessagesSubjects.ProcessingPlanState;
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        _ = this.loggerMock
            .Setup(m => m.IsEnabled(LogLevel.Debug))
            .Returns(value: true);
        var setupSequence = connectionMock
            .SetupSequence(m => m.SubscribeAsync(subject,
                It.IsAny<EventHandler<MsgHandlerEventArgs>>()));
        for (var i = 0; i < retries; i++)
        {
            _ = setupSequence.Throws<Exception>();
        }
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        natsConnection.TrySubscribe(subject, Mock.Of<EventHandler<MsgHandlerEventArgs>>(),
            Guid.NewGuid());

        // Assert
        this.loggerMock.VerifyLog(LogLevel.Error,
            message => message.Contains(subject) && message.Contains("Can't subscribe to subject"),
            () => Times.Exactly(retries));
        // To verify that it stopped at max retries
        this.loggerMock.VerifyLog(LogLevel.Error,
            message => message.Contains(subject) &&
                       message.Contains(NatsConnection.SubscribeMaxRetries.ToString()),
            Times.Once);
    }

    [TestMethod]
    public void TryToSubscribeRetriesAndReturnsIAsyncSubscriptionWhenSuccessfullySubscribed()
    {
        // Arrange
        const string subject = NatsMessagesSubjects.ProcessingPlanState;
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        _ = this.loggerMock
            .Setup(m => m.IsEnabled(LogLevel.Debug))
            .Returns(value: true);
        _ = connectionMock.SetupSequence(m => m.SubscribeAsync(subject,
                It.IsAny<EventHandler<MsgHandlerEventArgs>>()))
            .Throws<Exception>() // throw to see if the retry works
            .Returns(Mock.Of<IAsyncSubscription>());
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        natsConnection.TrySubscribe(subject, Mock.Of<EventHandler<MsgHandlerEventArgs>>(),
            Guid.NewGuid());

        // Assert
        this.loggerMock.VerifyLog(LogLevel.Debug,
            message => message.Contains(subject) && message.Contains("Try to subscribe"),
            () => Times.Exactly(callCount: 2));
        this.loggerMock.VerifyLog(LogLevel.Debug,
            message => message.Contains(subject) && message.Contains("Successfully subscribed"),
            Times.Once);
        this.loggerMock.VerifyLog(LogLevel.Error,
            message => message.Contains(subject) && message.Contains("Can't subscribe to subject"),
            Times.Once);
    }

    [TestMethod]
    public void TryToSubscribeReturnsIAsyncSubscriptionWhenSuccessfullySubscribed()
    {
        // Arrange
        const string subject = NatsMessagesSubjects.ProcessingPlanState;
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        _ = this.loggerMock
            .Setup(m => m.IsEnabled(LogLevel.Debug))
            .Returns(value: true);
        _ = connectionMock.Setup(m => m.SubscribeAsync(subject,
                It.IsAny<EventHandler<MsgHandlerEventArgs>>()))
            .Returns(Mock.Of<IAsyncSubscription>());
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        natsConnection.TrySubscribe(subject, Mock.Of<EventHandler<MsgHandlerEventArgs>>(),
            Guid.NewGuid());

        // Assert
        this.loggerMock.VerifyLog(LogLevel.Debug,
            message => message.Contains(subject) && message.Contains("Try to subscribe"),
            Times.Once);
        this.loggerMock.VerifyLog(LogLevel.Debug,
            message => message.Contains(subject) && message.Contains("Successfully subscribed"),
            Times.Once);
        this.loggerMock.Verify(LogLevel.Error, Times.Never());
    }

    [TestMethod]
    [DataRow(ConnState.CLOSED)]
    [DataRow(ConnState.CONNECTING)]
    [DataRow(ConnState.RECONNECTING)]
    [DataRow(ConnState.DISCONNECTED)]
    [DataRow(ConnState.DRAINING_PUBS)]
    [DataRow(ConnState.DRAINING_SUBS)]
    public void TryPublishDoesNotPublishWhenTheCurrentConnectionIsNotConnectedOrNull(ConnState state)
    {
        // Arrange
        const string subject = "abc";
        var data = Array.Empty<byte>();
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(state);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        var published = natsConnection.TryPublish(subject, data);

        // Assert
        Assert.IsFalse(published);
        connectionMock.Verify(m => m.Publish(subject, data), Times.Never);
    }

    [TestMethod]
    public void TryPublishDoesNotPublishWhenTheCurrentConnectionIsNull()
    {
        // Arrange
        const string subject = "abc";
        var data = Array.Empty<byte>();
        _ = this.connectionFactoryMock.SetupToReturnNullConnection(out var connectionOptionsMock);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged(new NatsOptions());

        // Act
        var published = natsConnection.TryPublish(subject, data);

        // Assert
        Assert.IsFalse(published);
    }

    [TestMethod]
    public void TryPublishReturnsFalseWhenConnectionPublishThrowsAnException()
    {
        // Arrange
        const string subject = "abc";
        var data = Array.Empty<byte>();
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        _ = connectionMock.Setup(m => m.Publish(subject, data)).Throws<Exception>();
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        var published = natsConnection.TryPublish(subject, data);

        // Assert
        Assert.IsFalse(published);
        connectionMock.Verify(m => m.Publish(subject, data), Times.Once);
        this.loggerMock.VerifyLog(LogLevel.Error,
            message => message.Contains(subject) && message.Contains("Can't publish"),
            Times.Once);
    }

    [TestMethod]
    public void TryPublishReturnsTrueWhenConnectedAndAbleToPublish()
    {
        // Arrange
        const string subject = "abc";
        var data = Array.Empty<byte>();
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();

        // Act
        var published = natsConnection.TryPublish(subject, data);

        // Assert
        Assert.IsTrue(published);
        connectionMock.Verify(m => m.Publish(subject, data), Times.Once);
        this.loggerMock.VerifyLog(LogLevel.Error,
            message => message.Contains(subject) && message.Contains("Can't publish"),
            Times.Never);
    }

    [TestMethod]
    public void UnsubscribeDisposesRegisteredSubscriptions()
    {
        // Arrange
        var receiverId = Guid.NewGuid();
        const string subject = "abc";
        _ = this.connectionFactoryMock.SetupAsDefault(out var connectionMock, out var connectionOptionsMock);
        _ = connectionMock.Setup(m => m.State).Returns(ConnState.CONNECTED);
        var subscription = new Mock<IAsyncSubscription>();
        _ = connectionMock.Setup(m => m.SubscribeAsync(subject,
                It.IsAny<EventHandler<MsgHandlerEventArgs>>()))
            .Returns(subscription.Object);
        using var natsConnection = this.CreateNatsConnection(connectionOptionsMock);
        _ = this.natsServerOptionsManagerMock.RaiseOptionsChanged();
        natsConnection.TrySubscribe(subject, It.IsAny<EventHandler<MsgHandlerEventArgs>>(),
            receiverId);

        // Act
        natsConnection.Unsubscribe(receiverId);

        // Assert
        subscription.Verify(m => m.Dispose(), Times.Once);
    }
}