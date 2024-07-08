using Microsoft.Extensions.Options;
using Moq;

namespace PROJECT_NAME_BusinessRulesTests.Extensions;

public static class MockExtensions
{
    public static Mock<IOptionsMonitor<TOptions>> SetupAsDefault<TOptions>(
        this Mock<IOptionsMonitor<TOptions>> optionsMonitorMock,
        TOptions? options = null) where TOptions : class, new()
    {
        optionsMonitorMock
            .Setup(mock => mock.CurrentValue)
            .Returns(options ?? new TOptions());
        return optionsMonitorMock;
    }
}