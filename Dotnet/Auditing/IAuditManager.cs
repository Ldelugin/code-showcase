namespace -- redacted --;

/// <summary>
/// A manager to save a <see cref="AuditInfo"/>.
/// </summary>
public interface IAuditManager
{
    /// <summary>
    /// Save the <paramref name="auditInfo"/>.
    /// </summary>
    /// <param name="auditInfo">
    /// The <see cref="AuditInfo"/> to save.
    /// </param>
    void SaveAudit(AuditInfo auditInfo);
}
