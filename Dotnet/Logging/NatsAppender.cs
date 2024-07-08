using System;
using -- redacted --;
using log4net.Appender;
using log4net.Core;

namespace -- redacted --;

/// <summary>
/// Custom implementation of an <see cref="AppenderSkeleton"/> to push logs as messages with a message broker.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="NatsAppender"/>.
/// </remarks>
/// <param name="messagePoster">The <see cref="IMessagePoster"/> that will be pushing the logs as messages.</param>
/// /// <param name="logLevelTranslator">Instance of <see cref="ILogLevelTranslator"/>.</param>
/// <param name="serviceName">The name of the service.</param>
/// <exception cref="ArgumentNullException">
/// Thrown when the provided <paramref name="messagePoster"/> is null or
/// thrown when the provided <paramref name="logLevelTranslator"/> is null.
/// </exception>
public class NatsAppender(IMessagePoster messagePoster, ILogLevelTranslator logLevelTranslator, ServiceNames serviceName) : AppenderSkeleton
{
    private readonly IMessagePoster messagePoster = messagePoster ?? throw new ArgumentNullException(nameof(messagePoster));
    private readonly ILogLevelTranslator logLevelTranslator = logLevelTranslator ?? throw new ArgumentNullException(nameof(logLevelTranslator));
    private readonly int processId = Environment.ProcessId;

    /// <summary>
    /// The name of the service.
    /// </summary>
    public ServiceNames ServiceName { get; } = serviceName;

    /// <summary>
    /// Subclasses of <see cref="T:log4net.Appender.AppenderSkeleton" /> should implement this method
    /// to perform actual logging.
    /// </summary>
    /// <param name="loggingEvent">The event to append.</param>
    /// <remarks>
    /// <para>
    /// A subclass must implement this method to perform
    /// logging of the <paramref name="loggingEvent" />.
    /// </para>
    /// <para>This method will be called by <see cref="M:DoAppend(LoggingEvent)" />
    /// if all the conditions listed for that method are met.
    /// </para>
    /// <para>
    /// To restrict the logging of events in the appender
    /// override the <see cref="M:PreAppendCheck()" /> method.
    /// </para>
    /// </remarks>
    /// <footer><a href="https://www.google.com/search?q=log4net.Appender.AppenderSkeleton.Append">`AppenderSkeleton.Append` on google.com</a></footer>
    protected override void Append(LoggingEvent loggingEvent)
    {
        var logLevel = this.logLevelTranslator.ToLogLevel(loggingEvent.Level);
        _ = this.messagePoster.PostLog(new Log(
            loggingEvent.MessageObject?.ToString(),
            loggingEvent.TimeStampUtc,
            this.processId,
            (byte)logLevel,
            (byte)this.ServiceName));
    }
}
