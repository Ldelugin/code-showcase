using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestsSupport;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsServerOptionsManagerTests
{
    private readonly Mock<IOptionsMonitor<NatsServerOptions>> natsServerOptionsMonitorMock = new();
    private readonly Mock<INatsConnectionOptions> natsConnectionOptionsMock = new();
    private readonly Mock<IEfSystemConfigurationProvider> entityFrameworkConfigurationProviderMock;
    private readonly Mock<IConfigurationRoot> configurationMock;
    private readonly Mock<ILogger<NatsServerOptionsManager>> loggerMock = new();
    private readonly Mock<ITimerFactory> timerFactoryMock = new();
    private readonly Mock<ITimer> timerMock = new();
    private readonly TestOptionsMonitor<NatsOptions> testOptionsMonitor = new(new NatsOptions());
    private readonly NatsServerOptionsManager natsServerOptionsManager;
    private readonly string rootKey;

    public NatsServerOptionsManagerTests()
    {
        this.entityFrameworkConfigurationProviderMock = new Mock<IEfSystemConfigurationProvider>();
        this.configurationMock = new Mock<IConfigurationRoot>();
        _ = this.configurationMock.Setup(root => root.Providers)
            .Returns([this.entityFrameworkConfigurationProviderMock.Object]);
        _ = this.timerFactoryMock.Setup(m => m.CreateTimer()).Returns(this.timerMock.Object);

        this.natsServerOptionsManager = new NatsServerOptionsManager(
            this.natsServerOptionsMonitorMock.Object,
            this.testOptionsMonitor,
            this.natsConnectionOptionsMock.Object,
            this.configurationMock.Object,
            this.loggerMock.Object,
            this.timerFactoryMock.Object);
        this.rootKey = ConfigurationPath.Combine(-- redacted --, NatsOptions.Name,
            NatsServerOptions.Name);
    }

    private static string[] GetOrderedRegisteredNames(IEnumerable<string> registeredNames, string localServer, int port)
    {
        return [.. registeredNames
            .Select(server => $"{NatsOptions.ListenUrlPrefix}{server}:{port}")
            .OrderByDescending(server => server.Contains($"{localServer}"))
            .ThenBy(server => server)];
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullException() =>
        Assert.That.ConstructorThrowsArgumentNullException<NatsServerOptionsManager>();

    [TestMethod]
    public void GetRegisteredServerNamesReturnsEmptyArrayIfNoSectionExists()
    {
        // Act
        var result = this.natsServerOptionsManager.GetRegisteredServerNames();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.SequenceEqual([]));
    }

    [TestMethod]
    [DataRow("Server01")]
    [DataRow("Server01", "Server02")]
    public void GetRegisteredServerNamesReturnsExpectedServerNames(params string[] registeredNames)
    {
        // Arrange
        _ = this.configurationMock.SetupGetChildrenWithStructure(this.rootKey, registeredNames);

        // Act
        var result = this.natsServerOptionsManager.GetRegisteredServerNames();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.SequenceEqual(registeredNames));
    }

    [TestMethod]
    public void UpdateTimestampReturnsFalseWhenTheTimestampIsLargerThanTheUpdateTimestamp()
    {
        // Arrange
        var natsServerOptions = new NatsServerOptions { Timestamp = DateTime.UtcNow };
        _ = this.natsServerOptionsMonitorMock
            .Setup(m => m.Get(Environment.MachineName))
            .Returns(natsServerOptions);
        var updateTimestamp = DateTime.UtcNow - TimeSpan.FromMinutes(value: 5);

        //Act
        var result = this.natsServerOptionsManager.UpdateTimestamp(updateTimestamp, out _);

        // Assert
        Assert.IsFalse(result);
    }

    public static IEnumerable<object[]> GetUpdateTimestamps()
    {
        yield return new object[] { DateTime.UtcNow - TimeSpan.FromMinutes(value: 8) };
        yield return new object[] { new DateTime() };
    }

    [TestMethod]
    [DynamicData(nameof(GetUpdateTimestamps), DynamicDataSourceType.Method)]
    public void UpdateTimestampReturnsTrueWhenTheTimestampIsSmallerThanTheUpdateTimestampAndSavesUpdatedTimestamp(
        DateTime timestamp)
    {
        // Arrange
        var serverName = Environment.MachineName;
        var natsServerOptions = new NatsServerOptions { Timestamp = timestamp };
        _ = this.natsServerOptionsMonitorMock
            .Setup(m => m.Get(serverName))
            .Returns(natsServerOptions);
        var updateTimestamp = DateTime.UtcNow - TimeSpan.FromMinutes(value: 5);

        // Act
        this.entityFrameworkConfigurationProviderMock.Reset();
        var result = this.natsServerOptionsManager.UpdateTimestamp(updateTimestamp, out _);

        // Assert
        Assert.IsTrue(result);
        this.entityFrameworkConfigurationProviderMock.VerifySetForNatsServerTimestamp(Times.Once, serverName,
            DateTime.UtcNow);
    }

    [TestMethod]
    public void IfNewServerUpdateTimestampOutParameterShouldReturnTrue()
    {
        // Arrange
        var natsServerOptions = new NatsServerOptions { Timestamp = new DateTime() };
        _ = this.natsServerOptionsMonitorMock
            .Setup(m => m.Get(Environment.MachineName))
            .Returns(natsServerOptions);
        var updateTimestamp = DateTime.UtcNow - TimeSpan.FromMinutes(value: 5);

        // Act
        _ = this.natsServerOptionsManager.UpdateTimestamp(updateTimestamp, out var isNewServer);

        // Assert
        Assert.IsTrue(isNewServer);
    }

    private static Dictionary<string, NatsServerOptions> GetServerNamesAndNatsServerOptions(int totalOutdatedServers)
    {
        totalOutdatedServers = totalOutdatedServers < 0 ? 0 : totalOutdatedServers;
        var dictionary = new Dictionary<string, NatsServerOptions>();
        var createdOutdatedServer = 0;

        for (var i = 0; i < 3 + totalOutdatedServers; i++)
        {
            var timestamp = DateTime.UtcNow;
            var postFix = string.Empty;

            if (totalOutdatedServers > 0 && createdOutdatedServer != totalOutdatedServers)
            {
                timestamp -= TimeSpan.FromHours(value: 2);
                postFix = "_outdated";
                createdOutdatedServer++;
            }

            dictionary.Add($"Server{i}{postFix}", new NatsServerOptions { Timestamp = timestamp });
        }

        return dictionary;
    }

    private void SetupNatsServerOptionsMonitor(Dictionary<string, NatsServerOptions> collection)
    {
        foreach (var (serverName, natsServerOptions) in collection)
        {
            _ = this.natsServerOptionsMonitorMock
                .Setup(m => m.Get(serverName))
                .Returns(natsServerOptions);
        }
    }

    [TestMethod]
    public void CheckForAndRemoveOutdatedServersReturnsFalseWhenNoServerIsOutdated()
    {
        // Arrange
        var collection = GetServerNamesAndNatsServerOptions(totalOutdatedServers: 0);
        this.SetupNatsServerOptionsMonitor(collection);
        _ = this.configurationMock.SetupGetChildrenWithStructure(this.rootKey, [.. collection.Keys]);

        // Act
        var outdatedTimestamp = DateTime.UtcNow - TimeSpan.FromHours(value: 1);
        var result = this.natsServerOptionsManager.CheckForAndRemoveOutdatedServers(outdatedTimestamp);

        // Assert
        Assert.IsFalse(result);
        this.entityFrameworkConfigurationProviderMock.VerifyDelete(Times.Never, keys: null);
    }

    [TestMethod]
    public void CheckForAndRemoveOutdatedServersReturnsTrueWhenDeletedOneOrMoreOutdatedServers()
    {
        // Arrange
        var collection = GetServerNamesAndNatsServerOptions(totalOutdatedServers: 2);
        this.SetupNatsServerOptionsMonitor(collection);
        _ = this.configurationMock.SetupGetChildrenWithStructure(this.rootKey, [.. collection.Keys]);

        // Act
        var outdatedTimestamp = DateTime.UtcNow - TimeSpan.FromHours(value: 1);
        var result = this.natsServerOptionsManager.CheckForAndRemoveOutdatedServers(outdatedTimestamp);

        // Assert
        Assert.IsTrue(result);
        var expectedDeletedServerTimestamps = collection.Keys
            .Where(name => name.EndsWith("_outdated"))
            .Select(name => name);
        foreach (var serverName in expectedDeletedServerTimestamps)
        {
            this.entityFrameworkConfigurationProviderMock.VerifyDelete(Times.Once, -- redacted --,
                NatsOptions.Name, NatsServerOptions.Name, serverName, nameof(NatsServerOptions.Timestamp));
        }
    }

    [TestMethod]
    public void CheckForAndRemoveOutdatedServersDoesNotThrowExceptionWhenDeleteFails()
    {
        // Arrange
        var collection = GetServerNamesAndNatsServerOptions(totalOutdatedServers: 2);
        this.SetupNatsServerOptionsMonitor(collection);
        _ = this.configurationMock.SetupGetChildrenWithStructure(this.rootKey, [.. collection.Keys]);
        _ = this.entityFrameworkConfigurationProviderMock
            .Setup(m => m.Delete(It.IsAny<string>()))
            .Throws<Exception>();

        // Act
        try
        {
            var outdatedTimestamp = DateTime.UtcNow - TimeSpan.FromHours(value: 1);
            _ = this.natsServerOptionsManager.CheckForAndRemoveOutdatedServers(outdatedTimestamp);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }

        // Assert
        this.loggerMock.Verify(LogLevel.Error, Times.Exactly(callCount: 2));
    }

    public static IEnumerable<object[]> GetDataForIsOptionsChangedExpectedValue()
    {
        const string user = "client";
        const string password = "secret";
        const int port = 5678;
        var localServer = Environment.MachineName;
        var registeredNames = new[] { "Server01", "Server02", localServer };
        var orderedRegisteredNames = GetOrderedRegisteredNames(registeredNames, localServer, port);
        var registeredNamesFromDatabase = registeredNames.Take(1..);
        var rootKey = ConfigurationPath.Combine(-- redacted --, NatsOptions.Name,
            NatsServerOptions.Name);

        var actionsThatShouldReflectThatTheOptionsAreChanged = new Dictionary<bool, (Action<NatsOptions>, Action<Mock<INatsConnectionOptions>, Mock<IConfigurationRoot>>)[]>
        {
            // These actions must trigger a new connection
            {true, new (Action<NatsOptions>, Action<Mock<INatsConnectionOptions>, Mock<IConfigurationRoot>>)[]
            {
                (options => options.NatsServerClientUserName = user, null),
                (options => options.NatsServerClientPassword = password, null),
                (options => options.NatsServerPort = port, null),
                (null, (natsConnectionOptionsMock, configurationMock) =>
                {
                    _ = natsConnectionOptionsMock.Setup(m => m.Servers).Returns((string[])null);
                    _ = configurationMock.SetupGetChildrenWithStructure(rootKey, registeredNamesFromDatabase);
                }),
                (null, (natsConnectionOptionsMock, configurationMock) =>
                {
                    _ = natsConnectionOptionsMock.Setup(m => m.Servers).Returns(orderedRegisteredNames);
                    _ = configurationMock.SetupGetChildrenWithStructure(rootKey, registeredNamesFromDatabase);
                })
            }},
            
            // These actions should not trigger a new connection
            {false, new (Action<NatsOptions>, Action<Mock<INatsConnectionOptions>, Mock<IConfigurationRoot>>)[]
            {
                (options => options.LogDebug = true, null),
                (options => options.LogTrace = true, null)
            }}
        };

        foreach (var kvp in actionsThatShouldReflectThatTheOptionsAreChanged)
        {
            foreach (var actions in kvp.Value)
            {
                yield return new object[] { actions.Item1, true, kvp.Key, actions.Item2 };
            }
        }

        // Expected that the options is not changed
        yield return new object[]
        {
            new Action<NatsOptions>(options =>
            {
                options.NatsServerClientUserName = user;
                options.NatsServerClientPassword = password;
                options.NatsServerPort = port;
            }),
            false, // expectedThatEventIsRaised
            null, //  event is not raised, so expectedToNeedConnection is null
            new Action<Mock<INatsConnectionOptions>, Mock<IConfigurationRoot>>((natsConnectionOptionsMock, configurationMock) =>
            {
                _ = natsConnectionOptionsMock.Setup(m => m.User).Returns(user);
                _ = natsConnectionOptionsMock.Setup(m => m.Password).Returns(password);
                _ = natsConnectionOptionsMock.Setup(m => m.Port).Returns(port);
                _ = natsConnectionOptionsMock.Setup(m => m.Servers).Returns(orderedRegisteredNames);
                _ = configurationMock.SetupGetChildrenWithStructure(rootKey, registeredNames);
            })
        };
    }

    [TestMethod]
    [DynamicData(nameof(GetDataForIsOptionsChangedExpectedValue), DynamicDataSourceType.Method)]
    public void TimerElapsedAndOptionsAreChangedSyncsOptionsAndRaiseOptionsChangedEvent(Action<NatsOptions> setupAction,
        bool expectedThatEventIsRaised, bool? expectedToNeedConnection,
        Action<Mock<INatsConnectionOptions>, Mock<IConfigurationRoot>> additionalSetupAction)
    {
        // Arrange
        var isOptionsChangedEventRaised = false;
        var natsOptions = this.testOptionsMonitor.CurrentValue;
        _ = this.natsConnectionOptionsMock.SetupAsDefault(natsOptions);
        setupAction?.Invoke(natsOptions);
        additionalSetupAction?.Invoke(this.natsConnectionOptionsMock, this.configurationMock);
        bool? isNewConnectionNeeded = null;

        void OptionsChanged(object o, NatsOptionsEventArgs natsOptionsEventArgs)
        {
            isOptionsChangedEventRaised = true;
            isNewConnectionNeeded = natsOptionsEventArgs.IsNewConnectionNeeded;
        }

        this.natsServerOptionsManager.OptionsChanged += OptionsChanged;

        // Act
        this.timerMock.Raise(m => m.Elapsed += null, new TimerEventArgs(state: null));

        // Assert
        this.natsServerOptionsManager.OptionsChanged -= OptionsChanged;
        Assert.AreEqual(expectedThatEventIsRaised, isOptionsChangedEventRaised);
        Assert.AreEqual(expectedToNeedConnection, isNewConnectionNeeded);
        var times = expectedThatEventIsRaised ? Times.Once() : Times.Never();
        this.natsConnectionOptionsMock.VerifySet(m => m.User = natsOptions.NatsServerClientUserName, times);
        this.natsConnectionOptionsMock.VerifySet(m => m.Password = natsOptions.NatsServerClientPassword, times);
        this.natsConnectionOptionsMock.VerifySet(m => m.Port = natsOptions.NatsServerPort, times);
        this.natsConnectionOptionsMock.VerifySet(m => m.LogDebug = natsOptions.LogDebug, times);
        this.natsConnectionOptionsMock.VerifySet(m => m.LogTrace = natsOptions.LogTrace, times);
    }

    [TestMethod]
    public void SyncOptionsFillsCorrectValuesToTheConnectionOptions()
    {
        // Arrange
        var localServer = Environment.MachineName;
        const string user = "user";
        const string password = "bla";
        const int port = 1234;
        const bool logDebug = true;
        const bool logTrace = true;
        var registeredNames = new[] { "Server01", "Server02", localServer };
        var natsOptions = new NatsOptions
        {
            NatsServerClientUserName = user,
            NatsServerClientPassword = password,
            NatsServerPort = port,
            LogDebug = logDebug,
            LogTrace = logTrace
        };
        _ = this.configurationMock.SetupGetChildrenWithStructure(this.rootKey, registeredNames);

        // Act
        this.testOptionsMonitor.Set(natsOptions);

        // Assert
        this.natsConnectionOptionsMock.VerifySet(m => m.User = user, Times.Once);
        this.natsConnectionOptionsMock.VerifySet(m => m.Password = password, Times.Once);
        this.natsConnectionOptionsMock.VerifySet(m => m.Port = port, Times.Once);
        this.natsConnectionOptionsMock.VerifySet(m => m.LogDebug = logDebug, Times.Once);
        this.natsConnectionOptionsMock.VerifySet(m => m.LogTrace = logTrace, Times.Once);
        var expectedOrderedRegisteredNames = GetOrderedRegisteredNames(registeredNames, localServer, port);
        this.natsConnectionOptionsMock.VerifySet(m =>
            m.Servers = expectedOrderedRegisteredNames, Times.Once);
    }

    [TestMethod]
    public void OnOptionsChangedSyncsOptionsAndRaisesOptionsChangedEvent()
    {
        // Arrange
        var isOptionsChangedEventRaised = false;
        var natsOptions = this.testOptionsMonitor.CurrentValue;
        void OptionsChanged(object o, NatsOptionsEventArgs natsOptionsEventArgs)
        {
            isOptionsChangedEventRaised = true;
        }
        this.natsServerOptionsManager.OptionsChanged += OptionsChanged;

        // Act
        this.testOptionsMonitor.Set(natsOptions);

        // Assert
        this.natsServerOptionsManager.OptionsChanged -= OptionsChanged;
        Assert.IsTrue(isOptionsChangedEventRaised);
        this.natsConnectionOptionsMock.VerifySet(m => m.User = natsOptions.NatsServerClientUserName, Times.Once());
        this.natsConnectionOptionsMock.VerifySet(m => m.Password = natsOptions.NatsServerClientPassword, Times.Once());
        this.natsConnectionOptionsMock.VerifySet(m => m.Port = natsOptions.NatsServerPort, Times.Once());
        this.natsConnectionOptionsMock.VerifySet(m => m.LogDebug = natsOptions.LogDebug, Times.Once());
        this.natsConnectionOptionsMock.VerifySet(m => m.LogTrace = natsOptions.LogTrace, Times.Once());
    }
}