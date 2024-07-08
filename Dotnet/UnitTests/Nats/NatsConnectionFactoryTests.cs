using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NATS.Client;
using TestsSupport;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsConnectionFactoryTests
{
    private readonly Mock<INatsConnectionOptions> natsConnectionOptionsMock = new();
    private readonly ConnectionFactory connectionFactory = new();
    private NatsConnectionFactory natsConnectionFactory;

    [TestMethod]
    public void ConstructorThrowsArgumentNullException()
        => Assert.That.ConstructorThrowsArgumentNullException<NatsConnectionFactory>();

    [TestMethod]
    public void CreateConnectionReturnsAConnectionThatIsConnectedToTheRunningNatsServer()
    {
        // Arrange
        _ = this.natsConnectionOptionsMock.SetupAsDefault();
        this.natsConnectionFactory = new NatsConnectionFactory(this.connectionFactory);
        using var server = NatsTestServer.Create();

        // Act
        using var connection = this.natsConnectionFactory.CreateConnection(this.natsConnectionOptionsMock.Object);

        // Assert
        Assert.IsNotNull(connection);
        Assert.AreEqual(ConnState.CONNECTED, connection.State);
    }
}