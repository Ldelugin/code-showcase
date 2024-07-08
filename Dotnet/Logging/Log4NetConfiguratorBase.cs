using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Configure Log4Net through code instead of a log4net.config file.
/// </summary>
public abstract class Log4NetConfiguratorBase : IDisposable
{
    private const string ConversionPatternFormat = "%date{ISO8601} [%logger:%line] %level - %message%newline%exception";
    private const string SimpleConversionPatternFormat = "%message%newline%exception";
    private const long MaximumFileSize = 10 * 1024 * 1024;
    private const string SystemAuditName = "SystemAudit";
    private const LogLevel DefaultLogLevel = LogLevel.None;

    private readonly IOptionsMonitor<LoggerFilterOptions> loggingOptionsMonitor;
    private readonly ILogLevelTranslator logLevelTranslator;
    private readonly ServiceNames serviceName;
    private readonly IDisposable changeToken;
    private readonly string logfilesDirectory;
    private IList<IAppender> appenders;

    /// <summary>
    /// Instantiates a new instance of <see cref="Log4NetConfigurator"/>.
    /// </summary>
    /// /// <param name="loggingOptionsMonitor">Instance of <see cref="IOptionsMonitor{LoggerFilterOptions}"/>.</param>
    /// /// <param name="logLevelTranslator">Instance of <see cref="ILogLevelTranslator"/>.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="logfilesDirectory">The directory to write the log files to.</param>
    /// <param name="appenders">The appenders to write logs to.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="loggingOptionsMonitor"/> is null or
    /// thrown when <paramref name="logLevelTranslator"/> is null.
    /// </exception>
    protected Log4NetConfiguratorBase(IOptionsMonitor<LoggerFilterOptions> loggingOptionsMonitor, ILogLevelTranslator logLevelTranslator,
        ServiceNames serviceName, string logfilesDirectory, IList<IAppender> appenders)
    {
        this.loggingOptionsMonitor = loggingOptionsMonitor ?? throw new ArgumentNullException(nameof(loggingOptionsMonitor));
        this.logLevelTranslator = logLevelTranslator ?? throw new ArgumentNullException(nameof(logLevelTranslator));
        this.serviceName = serviceName;
        this.logfilesDirectory = logfilesDirectory;
        this.appenders = appenders;

        this.changeToken = this.loggingOptionsMonitor.OnChange(this.OnLoggingOptionsChanged);
    }

    /// <summary>
    /// Dispose of the onchangeToken.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Configure Log4Net.
    /// </summary>
    public void Configure()
    {
        LogManager.ResetConfiguration();

        this.appenders ??= [];
        if (this.serviceName == ServiceNames.Portal)
        {
            this.appenders.Add(CreateSystemAuditRollingFileAppender(this.logfilesDirectory));
        }

        ConfigureAllRepositories(hierarchy =>
        {
            this.SetLevel(hierarchy, this.loggingOptionsMonitor.CurrentValue);
            hierarchy.Configured = true;
            _ = BasicConfigurator.Configure(hierarchy, [.. this.appenders]);
        });
    }

    /// <summary>
    /// Configure all the repositories.
    /// </summary>
    /// <param name="configure">Action to perform.</param>
    private static void ConfigureAllRepositories(Action<Hierarchy> configure)
    {
        var repositories = LogManager.GetAllRepositories();
        foreach (var repository in repositories)
        {
            var hierarchy = (Hierarchy)repository;
            configure.Invoke(hierarchy);
        }
    }

    /// <summary>
    /// Dispose of the changeToken.
    /// </summary>
    /// <param name="disposing">Whether this should dispose or not.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        this.changeToken?.Dispose();
    }

    /// <summary>
    /// Callback when the options is changed.
    /// </summary>
    /// <param name="options">Instance of <see cref="LoggerFilterOptions"/>.</param>
    /// <param name="name">The name of the instance.</param>
    private void OnLoggingOptionsChanged(LoggerFilterOptions options, string name) =>
        ConfigureAllRepositories(hierarchy => this.SetLevel(hierarchy, options));

    /// <summary>
    /// Set the Level on the <see cref="Hierarchy"/>.
    /// </summary>
    /// <param name="hierarchy">The instance of <see cref="Hierarchy"/>.</param>
    /// <param name="options">Instance of <see cref="LoggerFilterOptions"/>.</param>
    private void SetLevel(Hierarchy hierarchy, LoggerFilterOptions options)
    {
        var logLevel = options.Rules.FirstOrDefault(rule =>
            string.IsNullOrWhiteSpace(rule.CategoryName) &&
            rule.LogLevel != null)?.LogLevel ?? DefaultLogLevel;
        hierarchy.Root.Level = this.logLevelTranslator.ToLevel(logLevel);
        hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConsoleAppender"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="ConsoleAppender"/>.</returns>
    protected static ConsoleAppender CreateConsoleAppender()
    {
        var consoleAppender = new ConsoleAppender
        {
            Layout = new PatternLayout(ConversionPatternFormat)
        };
        consoleAppender.ActivateOptions();
        return consoleAppender;
    }

    /// <summary>
    /// Creates a new instance of <see cref="PatternString"/>.
    /// </summary>
    /// <param name="logFilesDirectory">The directory when the log files are placed.</param>
    /// <param name="fileName">The name of the log file.</param>
    /// <returns>A new instance of <see cref="PatternString"/>.</returns>
    private static PatternString CreateFilePatternString(string logFilesDirectory, string fileName)
        => new($"{logFilesDirectory}\\{fileName}.log");

    /// <summary>
    /// Creates the name of the log file without extension.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <returns>The name of the log file without extension.</returns>
    private static string CreateFileName(ServiceNames serviceName)
        => serviceName == ServiceNames.-- redacted -- ? $"{serviceName}" : $"-- redacted -- {serviceName} %processid";

    /// <summary>
    /// Creates a new instance of <see cref="RollingFileAppender"/>.
    /// </summary>
    /// <param name="logFilesDirectory">The directory when the log files are placed.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <returns>A new instance of <see cref="RollingFileAppender"/>.</returns>
    protected static RollingFileAppender CreateRollingFileAppender(string logFilesDirectory, ServiceNames serviceName)
    {
        var rollingFileAppender = new RollingFileAppender
        {
            File = CreateFilePatternString(logFilesDirectory, CreateFileName(serviceName)).Format(),
            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Size,
            MaxFileSize = MaximumFileSize,
            MaxSizeRollBackups = 5,
            StaticLogFileName = true,
            Layout = new PatternLayout(ConversionPatternFormat)
        };
        rollingFileAppender.AddFilter(new LoggerMatchFilter
        {
            LoggerToMatch = SystemAuditName,
            AcceptOnMatch = false
        });
        rollingFileAppender.ActivateOptions();
        return rollingFileAppender;
    }

    /// <summary>
    /// Creates a new instance of <see cref="RollingFileAppender"/> for the SystemAudit.
    /// </summary>
    /// <param name="logFilesDirectory">The directory when the log files are placed.</param>
    /// <returns>A new instance of <see cref="RollingFileAppender"/>.</returns>
    private static RollingFileAppender CreateSystemAuditRollingFileAppender(string logFilesDirectory)
    {
        var rollingFileAppender = new RollingFileAppender
        {
            File = CreateFilePatternString(logFilesDirectory, SystemAuditName).Format(),
            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Size,
            MaxFileSize = MaximumFileSize,
            MaxSizeRollBackups = 5,
            StaticLogFileName = true,
            Layout = new PatternLayout(SimpleConversionPatternFormat)
        };
        rollingFileAppender.AddFilter(new LoggerMatchFilter
        {
            LoggerToMatch = SystemAuditName
        });
        rollingFileAppender.AddFilter(new DenyAllFilter());
        rollingFileAppender.ActivateOptions();
        return rollingFileAppender;
    }

    /// <summary>
    /// Creates a new instance of <see cref="NatsAppender"/>.
    /// </summary>
    /// <param name="messagePoster">Instance of <see cref="messagePoster"/>.</param>
    /// /// <param name="logLevelTranslator">Instance of <see cref="ILogLevelTranslator"/>.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <returns>A new instance of <see cref="NatsAppender"/>.</returns>
    protected static NatsAppender CreateNatsAppender(IMessagePoster messagePoster, ILogLevelTranslator logLevelTranslator,
        ServiceNames serviceName)
    {
        var natsAppender = new NatsAppender(messagePoster, logLevelTranslator, serviceName)
        {
            Layout = new PatternLayout(ConversionPatternFormat)
        };
        natsAppender.ActivateOptions();
        return natsAppender;
    }
}