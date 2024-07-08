using System;

namespace -- redacted --;

/// <summary>
/// Defines a captured audit info.
/// </summary>
[Serializable]
public class AuditInfo
{
    /// <summary>
    /// The timestamp of the audit info captured. 
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// The identifier of the user that has performed the action.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// A textual description for this audit.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Creates a new <see cref="AuditInfo"/>.
    /// </summary>
    public AuditInfo()
    {
        this.Timestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// ToString override of the <see cref="AuditInfo"/>.
    /// </summary>
    /// <returns>
    /// Returns a custom string of all the internals.
    /// </returns>
    public override string ToString() => $"{this.Timestamp} - [user: {this.UserId}] {this.Description}";
}
