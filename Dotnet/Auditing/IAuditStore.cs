namespace -- redacted --;

/// <summary>
/// Used to define a different store to save <see cref="AuditInfo"/> to.
/// </summary>
public interface IAuditStore
{
    /// <summary>
    /// Save the <paramref name="auditInfo"/>.
    /// </summary>
    /// <param name="auditInfo">
    /// The <see cref="AuditInfo"/> to save.
    /// </param>
    bool SaveAudit(AuditInfo auditInfo);
}
