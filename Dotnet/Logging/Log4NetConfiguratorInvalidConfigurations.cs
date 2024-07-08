using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net.Appender;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Configure Log4Net through code instead of a log4net.config file, only log to a file and the console.
/// </summary>
/// <remarks>
/// Instantiates a new instance of <see cref="Log4NetConfiguratorInvalidConfigurations"/>.
/// </remarks>
/// /// <param name="loggingOptionsMonitor">Instance of <see cref="IOptionsMonitor{LoggerFilterOptions}"/>.</param>
/// /// <param name="logLevelTranslator">Instance of <see cref="ILogLevelTranslator"/>.</param>
/// <param name="serviceName">The name of the service.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when <paramref name="loggingOptionsMonitor"/> is null or
/// thrown when <paramref name="logLevelTranslator"/> is null.
/// </exception>
public sealed class Log4NetConfiguratorInvalidConfigurations(IOptionsMonitor<LoggerFilterOptions> loggingOptionsMonitor,
    ILogLevelTranslator logLevelTranslator, ServiceNames serviceName) : Log4NetConfiguratorBase(loggingOptionsMonitor, logLevelTranslator, serviceName, Assembly.GetExecutingAssembly().Location,
        new List<IAppender>
            {
                CreateConsoleAppender(),
                CreateRollingFileAppender(GetLogFilesDirectory(), serviceName)
            })
{

    /// <summary>
    /// First check if the log files directory is present at the normal location (in the installation directory).
    /// If it is present, log to that directory, otherwise log to the installation directory of the service.
    /// </summary>
    /// <returns>A string with the full path.</returns>
    private static string GetLogFilesDirectory()
    {
        var currentServiceFile = Assembly.GetExecutingAssembly().Location;
        var installationDirectory = Directory.GetParent(currentServiceFile)?.Parent;

        if (installationDirectory != null && Directory.Exists(Path.Combine(installationDirectory.FullName, "logs")))
        {
            return Path.Combine(installationDirectory.FullName, "logs");
        }

        return Path.GetDirectoryName(currentServiceFile);
    }
}