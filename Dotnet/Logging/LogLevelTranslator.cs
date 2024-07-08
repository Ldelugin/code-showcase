using System;
using log4net.Core;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="ILogLevelTranslator"/>.
/// </summary>
public class LogLevelTranslator : ILogLevelTranslator
{
    /// <summary>
    /// Get the correct <see cref="LogLevel"/> from the provided <see cref="Level"/>.
    /// </summary>
    /// <param name="level">The <see cref="Level"/> to translate.</param>
    /// <returns>The translated </returns>
    public LogLevel ToLogLevel(Level level)
    {
        if (level == Level.Critical || level == Level.Fatal)
        {
            return LogLevel.Critical;
        }

        if (level == Level.Debug)
        {
            return LogLevel.Debug;
        }

        if (level == Level.Error)
        {
            return LogLevel.Error;
        }

        if (level == Level.Info)
        {
            return LogLevel.Information;
        }

        if (level == Level.Trace)
        {
            return LogLevel.Trace;
        }

        return level == Level.Warn ? LogLevel.Warning : LogLevel.None;
    }

    /// <summary>
    /// Get the correct <see cref="Level"/> from the provided <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to translate.</param>
    /// <returns></returns>
    public Level ToLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => Level.Trace,
            LogLevel.Debug => Level.Debug,
            LogLevel.Information => Level.Info,
            LogLevel.Warning => Level.Warn,
            LogLevel.Error => Level.Error,
            LogLevel.Critical => Level.Critical,
            LogLevel.None => Level.Off,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, message: null)
        };
    }
}