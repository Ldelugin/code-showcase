using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSupport;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class NatsOptionsTests
{
    private static readonly Random Random = Random.Shared;

    private static IEnumerable<ServerAndTimestamp> GetRandomServerAndTimestamps(int count = 10)
        => Enumerable.Range(start: 0, count: count)
            .Select(i => new ServerAndTimestamp(
                $"Server{i:00}",
                Random.NextDateTime(DateTime.UtcNow, DateTimeGranularity.Minutes)));

    [TestMethod]
    public void ServerAndTimestampTrimsMillisecondsFromTimestamp()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var oldMillisecond = now.Millisecond;

        // Act
        var serverAndTimestamp = new ServerAndTimestamp("", now);

        // Assert
        if (oldMillisecond != 0)
        {
            Assert.AreNotEqual(oldMillisecond, serverAndTimestamp.Timestamp.Millisecond);
        }
        Assert.AreEqual(expected: 0, serverAndTimestamp.Timestamp.Millisecond);
    }

    public static IEnumerable<object[]> ToStringData()
    {
        foreach (var element in GetRandomServerAndTimestamps())
        {
            yield return new object[] { element, element.Timestamp.ToString(CultureInfo.InvariantCulture) };
        }
    }

    [TestMethod]
    [DynamicData(nameof(ToStringData), DynamicDataSourceType.Method)]
    public void ServerAndTimestampTimestampAsStringReturnsExpectedResult(ServerAndTimestamp serverAndTimestamp, string expected)
    {
        // Act
        var result = serverAndTimestamp.TimestampAsString;

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ServerAndTimestampUpdateTimestampReturnsExpectedResult()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var serverAndTimestamp = new ServerAndTimestamp("", now);
        Assert.AreEqual(now.Trim(ServerAndTimestamp.TrimInterval), serverAndTimestamp.Timestamp);

        // Act
        var newTimestamp = now.AddHours(value: 2);
        serverAndTimestamp.UpdateTimestamp(newTimestamp);

        // Assert
        Assert.AreEqual(newTimestamp.Trim(ServerAndTimestamp.TrimInterval), serverAndTimestamp.Timestamp);
    }

    [TestMethod]
    [DataRow("Server01", 1234, null, "Server01:1234")]
    [DataRow("Server99", 9876, NatsOptions.ListenUrlPrefix, NatsOptions.ListenUrlPrefix + "Server99:9876")]
    public void CreateUrlReturnsExpectedResult(string name, int port, string prefix, string expected)
    {
        // Act
        var result = NatsOptions.CreateUrl(name, port, prefix);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(1234, "0.0.0.0:1234")]
    [DataRow(4222, "0.0.0.0:4222")]
    public void CreateListenUrlReturnsExpectedResult(int port, string expected)
    {
        // Arrange
        var options = new NatsOptions { NatsServerPort = port };

        // Act
        var result = options.CreateListenUrl();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("Server42", 9999, NatsOptions.RouteUrlPrefix + "Server42:9999")]
    [DataRow("localhost", 1234, NatsOptions.RouteUrlPrefix + "localhost:1234")]
    public void CreateRouteUrlReturnsExpectedResult(string name, int port, string expected)
    {
        // Arrange
        var options = new NatsOptions { RoutePort = port };

        // Act
        var result = options.CreateRouteUrl(name);

        // Assert
        Assert.AreEqual(expected, result);
    }
}