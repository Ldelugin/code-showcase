using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestsSupport;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsConfigurationFileCreatorTests
{
    private static readonly Random Random = Random.Shared;
    private static IEnumerable<ServerAndTimestamp> GetRandomServerAndTimestamps(int count = 10)
        => Enumerable.Range(start: 0, count: count)
            .Select(i => new ServerAndTimestamp(
                $"Server{i:00}",
                Random.NextDateTime(DateTime.UtcNow, DateTimeGranularity.Minutes)));

    private const string ExpectedTemplate =
        NatsConfigurationFileCreator.LogDebugFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.LogTraceFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.LogFileFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.ListenFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.PortFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.HttpFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.AuthorizationFormat + NatsConfigurationFileCreator.LineEnding +
        NatsConfigurationFileCreator.ClusterFormat + NatsConfigurationFileCreator.LineEnding;

    [TestMethod]
    public void GetTemplateReturnsExpectedResult()
    {
        // Act
        var result = NatsConfigurationFileCreator.GetTemplate();

        // Assert
        Assert.AreEqual(ExpectedTemplate, result);
    }

    [TestMethod]
    [DataRow("Server01", "Server02")]
    [DataRow("OtherServer", "AnotherServer")]
    public void ConstructorSetsCorrectServerName(string serverName, string otherServerName)
    {
        // Arrange
        var options = new NatsOptions();
        var natsServerOptionsManager = new Mock<INatsServerOptionsManager>();
        _ = natsServerOptionsManager
            .Setup(n => n.GetRegisteredServerNames())
            .Returns([serverName, otherServerName]);

        // Act
        var result = NatsConfigurationFileCreator.Create(options, serverName, natsServerOptionsManager.Object);

        // Assert
        Assert.IsFalse(result.Contains(serverName));
        Assert.IsTrue(result.Contains(otherServerName));
    }

    public static IEnumerable<object[]> GetData()
    {
        var natsServerOptionsManagerMock = new Mock<INatsServerOptionsManager>();
        _ = natsServerOptionsManagerMock.Setup(m => m.GetRegisteredServerNames())
            .Returns([]);

        yield return new object[]
        {
            new NatsOptions{ LogFile = @"c:\\path\nats-log-file.log" },
            NatsConfigurationFileCreator.LogFileTemplate,
            @"c:\\path\nats-log-file.log",
            natsServerOptionsManagerMock.Object
        };

        foreach (var boolean in new[] { true, false })
        {
            yield return new object[]
            {
                new NatsOptions { LogDebug = boolean },
                NatsConfigurationFileCreator.LogDebugTemplate,
                NatsConfigurationFileCreator.LogDebugFormat.Replace(NatsConfigurationFileCreator.LogDebugTemplate,
                    boolean.ToString().ToLower()),
                natsServerOptionsManagerMock.Object
            };

            yield return new object[]
            {
                new NatsOptions { LogTrace = boolean },
                NatsConfigurationFileCreator.LogTraceTemplate,
                NatsConfigurationFileCreator.LogTraceFormat.Replace(NatsConfigurationFileCreator.LogTraceTemplate,
                    boolean.ToString().ToLower()),
                natsServerOptionsManagerMock.Object
            };
        }

        yield return new object[]
        {
            new NatsOptions{  NatsServerPort = 1234 },
            NatsConfigurationFileCreator.ListenUrlTemplate,
            NatsOptions.CreateUrl("0.0.0.0", port: 1234),
            natsServerOptionsManagerMock.Object
        };

        yield return new object[]
        {
            new NatsOptions{ NatsServerPort = 1234 },
            NatsConfigurationFileCreator.PortTemplate,
            "1234",
            natsServerOptionsManagerMock.Object
        };

        yield return new object[]
        {
            new NatsOptions{ MonitorPort = 5555 },
            NatsConfigurationFileCreator.MonitorPortTemplate,
            "5555",
            natsServerOptionsManagerMock.Object
        };

        yield return new object[]
        {
            new NatsOptions{ NatsServerClientUserName = "test-user-name" },
            NatsConfigurationFileCreator.UserNameTemplate,
            "test-user-name",
            natsServerOptionsManagerMock.Object
        };

        yield return new object[]
        {
            new NatsOptions{ NatsServerClientUserName = "test-user-password" },
            NatsConfigurationFileCreator.PasswordTemplate,
            "test-user-password",
            natsServerOptionsManagerMock.Object
        };

        var serverAndTimestamps = GetRandomServerAndTimestamps(count: 2).ToArray();
        natsServerOptionsManagerMock.Reset();
        _ = natsServerOptionsManagerMock.Setup(m => m.GetRegisteredServerNames())
            .Returns(serverAndTimestamps.Select(s => s.Server));

        var optionsWithMultipleServers = new NatsOptions
        {
            ClusterName = "test-cluster-name",
            RoutePort = 9876
        };

        yield return new object[]
        {
            optionsWithMultipleServers,
            NatsConfigurationFileCreator.ClusterNameTemplate,
            "test-cluster-name",
            natsServerOptionsManagerMock.Object
        };

        yield return new object[]
        {
            optionsWithMultipleServers,
            NatsConfigurationFileCreator.RoutesPortTemplate,
            "9876",
            natsServerOptionsManagerMock.Object
        };

        var allRoutes = serverAndTimestamps.Select(st =>
            optionsWithMultipleServers.CreateRouteUrl(st.Server));

        yield return new object[]
        {
            optionsWithMultipleServers,
            NatsConfigurationFileCreator.AllRoutesTemplate,
            string.Join(NatsConfigurationFileCreator.LineEnding, allRoutes),
            natsServerOptionsManagerMock.Object
        };
    }

    [TestMethod]
    [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
    public void CreateReplacesAllTemplatesWithCorrectValues(NatsOptions options, string templateToReplace,
        string expectedValue, INatsServerOptionsManager natsServerOptionsManager)
    {
        // Arrange
        var template = NatsConfigurationFileCreator.GetTemplate();
        Assert.IsTrue(template.Contains(templateToReplace));
        Assert.IsFalse(template.Contains(expectedValue));

        // Act
        var result = NatsConfigurationFileCreator.Create(options, Environment.MachineName, natsServerOptionsManager);

        // Assert
        Assert.IsFalse(result.Contains(templateToReplace));
        Assert.IsTrue(result.Contains(expectedValue));
    }
}