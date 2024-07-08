using System;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NATS.Client;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsConnectionOptionsTests
{
    private readonly Options options;
    private readonly NatsConnectionOptions natsConnectionOptions;

    public NatsConnectionOptionsTests()
    {
        this.options = ConnectionFactory.GetDefaultOptions();
        this.natsConnectionOptions = new NatsConnectionOptions(this.options);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ConstructorThrowsArgumentNullException() => _ = new NatsConnectionOptions(options: null);

    [TestMethod]
    public void ConstructorSetsNoRandomizeToTrue()
    {
        // Arrange
        var innerOptions = ConnectionFactory.GetDefaultOptions();
        Assert.IsFalse(innerOptions.NoRandomize);

        // Act
        _ = new NatsConnectionOptions(innerOptions);

        // Assert
        Assert.IsTrue(innerOptions.NoRandomize);
    }

    [TestMethod]
    [DataRow(Defaults.Port, Defaults.Port)]
    [DataRow(1234, 1234)]
    public void PortGetsAndSetsCorrectValue(int port, int expected)
    {
        // Arrange
        Assert.AreEqual(expected: 0, this.natsConnectionOptions.Port);

        // Act
        this.natsConnectionOptions.Port = port;

        // Assert
        Assert.AreEqual(expected, this.natsConnectionOptions.Port);
    }

    [TestMethod]
    public void UserGetsTheCorrectValue()
    {
        // Arrange
        const string user = "-- redacted --";
        Assert.IsNull(this.natsConnectionOptions.User);
        this.options.User = user;

        // Act
        var actualUser = this.natsConnectionOptions.User;

        // Assert
        Assert.AreEqual(user, actualUser);
    }

    [TestMethod]
    public void UserSetsTheCorrectValue()
    {
        // Arrange
        const string user = "-- redacted --";
        Assert.IsNull(this.options.User);

        // Act
        this.natsConnectionOptions.User = user;

        // Assert
        Assert.AreEqual(user, this.options.User);
    }

    [TestMethod]
    public void ServersGetsTheCorrectValue()
    {
        // Arrange
        var servers = new[] { "nats://Server01", "nats://Server02" };
        Assert.IsNull(this.natsConnectionOptions.Servers);
        this.options.Servers = servers;

        // Act
        var actualServers = this.natsConnectionOptions.Servers;

        // Assert
        CollectionAssert.AreEqual(servers, actualServers);
    }

    [TestMethod]
    public void ServersSetsTheCorrectValue()
    {
        // Arrange
        var servers = new[] { "nats://Server01", "nats://Server02" };
        Assert.IsNull(this.natsConnectionOptions.Servers);

        // Act
        this.natsConnectionOptions.Servers = servers;

        // Assert
        CollectionAssert.AreEqual(servers, this.options.Servers);
    }

    [TestMethod]
    public void PasswordGetsAndSetsTheCorrectValue()
    {
        // Arrange
        const string password = "ABC123!";
        // The property Password of the Options class from Nats does not have a getter, so we can't assert that here.
        Assert.IsNull(this.natsConnectionOptions.Password);

        // Act
        this.natsConnectionOptions.Password = password;

        // Assert
        // Only way to verify the password is to check this.natsConnectionOptions.Password.
        Assert.AreEqual(password, this.natsConnectionOptions.Password);
    }

    [TestMethod]
    public void InternalNatsOptionsReturnsCorrectResult()
    {
        // Act
        var actualInstance = this.natsConnectionOptions.InternalNatsOptions;

        // Assert
        Assert.AreEqual(this.options, actualInstance);
    }

    [TestMethod]
    public void ClosedEventHandlerIsAddedToTheInnerOptions()
    {
        // Arrange
        this.options.ClosedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnClosed));

        // Act
        this.natsConnectionOptions.OnClosed += this.OnClosed;

        // Assert
        this.options.ClosedEventHandler.HasInvocation(this, nameof(this.OnClosed));
    }

    [TestMethod]
    public void ClosedEventHandlerIsRemovedFromTheInnerOptions()
    {
        // Arrange
        this.natsConnectionOptions.OnClosed += this.OnClosed;
        this.options.ClosedEventHandler.HasInvocation(this, nameof(this.OnClosed));

        // Act
        this.natsConnectionOptions.OnClosed -= this.OnClosed;

        // Assert
        this.options.ClosedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnClosed));
    }

    [TestMethod]
    public void DisconnectedEventHandlerIsAddedToTheInnerOptions()
    {
        // Arrange
        this.options.DisconnectedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnDisconnected));

        // Act
        this.natsConnectionOptions.OnDisconnected += this.OnDisconnected;

        // Assert
        this.options.DisconnectedEventHandler.HasInvocation(this, nameof(this.OnDisconnected));
    }

    [TestMethod]
    public void DisconnectedEventHandlerIsRemovedFromTheInnerOptions()
    {
        // Arrange
        this.natsConnectionOptions.OnDisconnected += this.OnDisconnected;
        this.options.DisconnectedEventHandler.HasInvocation(this, nameof(this.OnDisconnected));

        // Act
        this.natsConnectionOptions.OnDisconnected -= this.OnDisconnected;

        // Assert
        this.options.DisconnectedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnDisconnected));
    }

    [TestMethod]
    public void ReconnectedEventHandlerIsAddedToTheInnerOptions()
    {
        // Arrange
        this.options.ReconnectedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnReconnected));

        // Act
        this.natsConnectionOptions.OnReconnected += this.OnReconnected;

        // Assert
        this.options.ReconnectedEventHandler.HasInvocation(this, nameof(this.OnReconnected));
    }

    [TestMethod]
    public void ReconnectedEventHandlerIsRemovedFromTheInnerOptions()
    {
        // Arrange
        this.natsConnectionOptions.OnReconnected += this.OnReconnected;
        this.options.ReconnectedEventHandler.HasInvocation(this, nameof(this.OnReconnected));

        // Act
        this.natsConnectionOptions.OnReconnected -= this.OnReconnected;

        // Assert
        this.options.ReconnectedEventHandler.DoesNotHaveInvocation(this, nameof(this.OnReconnected));
    }

    [TestMethod]
    public void ServerDiscoveredEventHandlerIsAddedToTheInnerOptions()
    {
        // Arrange
        this.options.ServerDiscoveredEventHandler.DoesNotHaveInvocation(this, nameof(this.OnServerDiscovered));

        // Act
        this.natsConnectionOptions.OnServerDiscovered += this.OnServerDiscovered;

        // Assert
        this.options.ServerDiscoveredEventHandler.HasInvocation(this, nameof(this.OnServerDiscovered));
    }

    [TestMethod]
    public void ServerDiscoveredEventHandlerIsRemovedFromTheInnerOptions()
    {
        // Arrange
        this.natsConnectionOptions.OnServerDiscovered += this.OnServerDiscovered;
        this.options.ServerDiscoveredEventHandler.HasInvocation(this, nameof(this.OnServerDiscovered));

        // Act
        this.natsConnectionOptions.OnServerDiscovered -= this.OnServerDiscovered;

        // Assert
        this.options.ServerDiscoveredEventHandler.DoesNotHaveInvocation(this, nameof(this.OnServerDiscovered));
    }

    [TestMethod]
    public void AsyncErrorEventHandlerIsAddedToTheInnerOptions()
    {
        // Arrange
        this.options.AsyncErrorEventHandler.DoesNotHaveInvocation(this, nameof(this.OnError));

        // Act
        this.natsConnectionOptions.OnError += this.OnError;

        // Assert
        this.options.AsyncErrorEventHandler.HasInvocation(this, nameof(this.OnError));
    }

    [TestMethod]
    public void AsyncErrorEventHandlerIsRemovedFromTheInnerOptions()
    {
        // Arrange
        this.natsConnectionOptions.OnError += this.OnError;
        this.options.AsyncErrorEventHandler.HasInvocation(this, nameof(this.OnError));

        // Act
        this.natsConnectionOptions.OnError -= this.OnError;

        // Assert
        this.options.AsyncErrorEventHandler.DoesNotHaveInvocation(this, nameof(this.OnError));
    }

    private void OnDisconnected(object sender, ConnEventArgs eventArgs)
    {
    }

    private void OnReconnected(object sender, ConnEventArgs eventArgs)
    {
    }

    private void OnServerDiscovered(object sender, ConnEventArgs eventArgs)
    {
    }

    private void OnClosed(object sender, ConnEventArgs eventArgs)
    {
    }

    private void OnError(object sender, ErrEventArgs eventArgs)
    {
    }
}