using System;
using System.Collections.Generic;
using -- redacted --;
using -- redacted --;
using log4net.Appender;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Configure Log4Net through code instead of a log4net.config file.
/// </summary>
/// <remarks>
/// Instantiates a new instance of <see cref="Log4NetConfigurator"/>.
/// </remarks>
/// <param name="messagePoster">Instance of <see cref="messagePoster"/>.</param>
/// <param name="optionsMonitor">Instance of <see cref="IOptionsMonitor{-- redacted --}"/>.</param>
/// /// <param name="loggingOptionsMonitor">Instance of <see cref="IOptionsMonitor{LoggerFilterOptions}"/>.</param>
/// /// <param name="logLevelTranslator">Instance of <see cref="ILogLevelTranslator"/>.</param>
/// <param name="serviceName">The name of the service.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="messagePoster"/> is null or
/// thrown when <paramref name="optionsMonitor"/> is null or
/// thrown when <paramref name="loggingOptionsMonitor"/> is null or
/// thrown when <paramref name="logLevelTranslator"/> is null.
/// </exception>
public sealed class Log4NetConfigurator(IMessagePoster messagePoster, IOptionsMonitor<-- redacted --> optionsMonitor,
    IOptionsMonitor<LoggerFilterOptions> loggingOptionsMonitor, ILogLevelTranslator logLevelTranslator,
    ServiceNames serviceName) : Log4NetConfiguratorBase(loggingOptionsMonitor, logLevelTranslator, serviceName,
        optionsMonitor.CurrentValue.LoggingOptions.LogFilesDirectory,
        new List<IAppender>
            {
                CreateConsoleAppender(),
                CreateRollingFileAppender(optionsMonitor.CurrentValue.LoggingOptions.LogFilesDirectory,
                    serviceName),
                CreateNatsAppender(messagePoster, logLevelTranslator, serviceName)
            })
{
}