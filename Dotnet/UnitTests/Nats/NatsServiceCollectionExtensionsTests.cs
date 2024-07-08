using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NATS.Client;
using TestsSupport.Extensions;
using TestsSupport.TestClasses;
using Options = NATS.Client.Options;

namespace -- redacted --;

[TestClass]
public class NatsServiceCollectionExtensionsTests : ServiceCollectionTestBase
{
    private readonly Mock<IConfigurationSection> -- redacted --OptionsConfigurationMock = new();

    public NatsServiceCollectionExtensionsTests()
    {
        var entityFrameworkConfigurationProviderMock = new Mock<IEfSystemConfigurationProvider>();
        _ = this.ConfigurationRootMock.Setup(root => root.Providers)
            .Returns([entityFrameworkConfigurationProviderMock.Object]);

        _ = this.ServiceCollection
            .AddSingleton<IConfiguration>(this.ConfigurationRootMock.Object)
            .AddLogging()
            .AddOptions()
            .AddSingleton(Mock.Of<IEfSystemConfigurationProvider>());
        _ = this.ConfigurationRootMock
            .SetupGetSection(this.-- redacted --OptionsConfigurationMock, -- redacted --, NatsOptions.Name)
            .SetupGetSection(-- redacted --, NatsOptions.Name, NatsServerOptions.Name);
        _ = this.ConfigurationMock
            .SetupGetSection(this.-- redacted --OptionsConfigurationMock, -- redacted --, NatsOptions.Name)
            .SetupGetSection(-- redacted --, NatsOptions.Name, NatsServerOptions.Name);
    }

    [TestMethod]
    public void ConnectionFactoryIsRegistered() =>
        this.AssertServiceIsRegistered<ConnectionFactory>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void OptionsIsRegistered() =>
        this.AssertServiceIsRegistered<Options>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void OptionsIsRegisteredWithConfiguredInstance()
    {
        // Arrange
        const string name = "abc";
        static void Configure(Options options)
        {
            options.Name = name;
        }

        var options = this.AssertServiceIsRegistered<Options>(services =>
        {
            _ = services.AddNats(this.ConfigurationRootMock.Object, Configure);
        });

        // Assert
        Assert.AreEqual(name, options.Name);
    }

    [TestMethod]
    public void NatsConnectionFactoryIsRegistered() =>
        this.AssertServiceIsRegistered<INatsConnectionFactory>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void NatsConnectionOptionsIsRegistered() =>
        this.AssertServiceIsRegistered<INatsConnectionOptions>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void NatsConfigurationIsRegistered() =>
        this.AssertServiceIsRegistered<INatsConnection>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void TimerFactoryIsRegistered() =>
        this.AssertServiceIsRegistered<ITimerFactory>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void MessageReceiverIsRegistered() =>
        this.AssertServiceIsRegistered<IMessageReceiver>(services =>
        {
            _ = services.AddNats(this.ConfigurationRootMock.Object);
            _ = services.AddNatsMessageReceiver();
        });

    [TestMethod]
    public void MessagePosterIsRegistered() =>
        this.AssertServiceIsRegistered<IMessagePoster>(services =>
        {
            _ = services.AddNats(this.ConfigurationRootMock.Object);
            _ = services.AddNatsMessagePoster();
        });

    [TestMethod]
    public void NatsServerOptionsManagerIsRegistered() =>
        this.AssertServiceIsRegistered<INatsServerOptionsManager>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void NatsOptionsChangeTokenSourceIsRegistered() =>
        this.AssertServiceIsRegistered<IOptionsChangeTokenSource<NatsOptions>>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void NatsOptionsNamedConfigureFromConfigurationOptionsIsRegistered() =>
        this.AssertServiceIsRegistered<IConfigureOptions<NatsOptions>>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));

    [TestMethod]
    public void NatsServerOptionsNamedConfigureFromConfigurationOptionsIsRegistered() =>
        this.AssertServiceIsRegistered<IConfigureOptions<NatsServerOptions>>(services =>
           services.AddNats(this.ConfigurationRootMock.Object));
}