using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// The implementation of <see cref="IAuditManager"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="AuditManager"/>.
/// </remarks>
/// <param name="logger">
/// The logger instance.
/// </param>
/// <param name="configurations">
/// The AuditConfigurations instance.
/// </param>
/// <exception cref="ArgumentNullException">
/// When <paramref name="logger"/> is null.
/// </exception>
public class AuditManager(ILogger<AuditManager> logger, IEnumerable<IAuditStore> auditStores) : IAuditManager
{
    /// <summary>
    /// A list with all the registered audit stores.
    /// </summary>
    private readonly IEnumerable<IAuditStore> auditStores = auditStores ?? throw new ArgumentNullException(nameof(auditStores));

    /// <summary>
    /// The logger instance.
    /// </summary>
    private readonly ILogger<AuditManager> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Saves the provided <see cref="AuditInfo"/>.
    /// </summary>
    /// <param name="auditInfo">
    /// The <see cref="AuditInfo"/> to save.
    /// </param>
    public void SaveAudit(AuditInfo auditInfo)
    {
        if (auditInfo == null)
        {
            this.logger.LogWarning("Can not save a audit info that is null");
            return;
        }

        if (!this.auditStores.Any())
        {
            this.logger.LogWarning("Can not save the audit info due to no audit stores being registered");
            return;
        }

        foreach (var auditStore in this.auditStores)
        {
            var saved = auditStore.SaveAudit(auditInfo);
            this.logger.OnlyInDebug(l => l.LogDebug("  - to audit store: {AuditStore} - {Status}", auditStore,
                saved ? "successfully" : "unsuccessfully"));
        }
    }
}
