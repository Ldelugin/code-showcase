using System;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Event arguments that contains a <see cref="NatsOptions"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="NatsOptionsEventArgs"/>.
/// </remarks>
/// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
/// <param name="isNewConnectionNeeded">Indicates whether a new connection is needed.</param>
public class NatsOptionsEventArgs(NatsOptions natsOptions, bool isNewConnectionNeeded) : EventArgs
{

    /// <summary>
    /// Instance of <see cref="NatsOptions"/>.
    /// </summary>
    public NatsOptions NatsOptions { get; } = natsOptions;

    /// <summary>
    /// Indicates whether a new connection is needed.
    /// </summary>
    public bool IsNewConnectionNeeded { get; } = isNewConnectionNeeded;
}