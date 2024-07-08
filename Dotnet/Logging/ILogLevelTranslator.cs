using log4net.Core;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// Describes a contract to translate a <see cref="LogLevel"/> to <see cref="Level"/> and a
/// <see cref="Level"/> to <see cref="LogLevel"/>
/// </summary>
public interface ILogLevelTranslator
{
    /// <summary>
    /// Get the correct <see cref="LogLevel"/> from the provided <see cref="Level"/>.
    /// </summary>
    /// <param name="level">The <see cref="Level"/> to translate.</param>
    /// <returns>The translated </returns>
    LogLevel ToLogLevel(Level level);

    /// <summary>
    /// Get the correct <see cref="Level"/> from the provided <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to translate.</param>
    /// <returns></returns>
    Level ToLevel(LogLevel logLevel);
}