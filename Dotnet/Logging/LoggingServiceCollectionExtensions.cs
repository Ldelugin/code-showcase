using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Extension method(s) for the <see cref="IServiceCollection"/> to add configurators for Log4Net.
/// </summary>
public static class LoggingServiceCollectionExtensions
{
    /// <summary>
    /// Add the Log4NetConfigurations.
    /// </summary>
    /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/>.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <returns>The <paramref name="serviceCollection"/> to support method chaining.</returns>
    public static IServiceCollection AddLog4NetConfigurators(this IServiceCollection serviceCollection,
        ServiceNames serviceName)
    {
        serviceCollection.TryAddSingleton<ILogLevelTranslator, LogLevelTranslator>();
        serviceCollection.TryAddSingleton(serviceProvider =>
        {
            var loggingOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
            var logLevelTranslator = serviceProvider.GetRequiredService<ILogLevelTranslator>();
            return new Log4NetConfiguratorInvalidConfigurations(loggingOptionsMonitor, logLevelTranslator, serviceName);
        });

        serviceCollection.TryAddSingleton(serviceProvider =>
            {
                var messagePoster = serviceProvider.GetRequiredService<IMessagePoster>();
                var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<-- redacted -->>();
                var loggingOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>();
                var logLevelTranslator = serviceProvider.GetRequiredService<ILogLevelTranslator>();
                return new Log4NetConfigurator(messagePoster, optionsMonitor, loggingOptionsMonitor,
                    logLevelTranslator, serviceName);
            });
        return serviceCollection;
    }
}