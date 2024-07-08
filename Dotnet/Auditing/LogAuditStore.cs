using log4net;

namespace -- redacted --;

/// <summary>
/// Saves the <see cref="AuditInfo"/> to a log file.
/// </summary>
public class LogAuditStore : IAuditStore
{
    /// <summary>
    /// The name of the logger.
    /// </summary>
    private const string LoggerName = "SystemAudit";

    /// <summary>
    /// The logger instance.
    /// </summary>
    private readonly ILog logger;

    /// <summary>
    /// Creates a new <see cref="LogAuditStore"/>.
    /// </summary>
    public LogAuditStore()
    {
        this.logger = LogManager.GetLogger(LoggerName);
    }

    /// <summary>
    /// Save the <paramref name="auditInfo"/> to a logfile.
    /// </summary>
    /// <param name="auditInfo">
    /// The <see cref="AuditInfo"/> to save.
    /// </param>
    public bool SaveAudit(AuditInfo auditInfo)
    {
        if (auditInfo == null)
        {
            return false;
        }

        this.logger.Info(auditInfo.ToString());
        return true;
    }
}
