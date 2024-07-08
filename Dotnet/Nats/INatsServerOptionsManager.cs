using System;
using System.Collections.Generic;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Interface that describes the contract for managing the Nats server options.
/// </summary>
public interface INatsServerOptionsManager : IDisposable
{
    /// <summary>
    /// Event to report that the NatsOptions is changed.
    /// </summary>
    event EventHandler<NatsOptionsEventArgs> OptionsChanged;

    /// <summary>
    /// Update the timestamp for the server if the timestamp of the server is smaller then the
    /// given <paramref name="updateTimestamp"/>.
    /// </summary>
    /// <param name="updateTimestamp">The <see cref="DateTime"/> to compare against.</param>
    /// <param name="isNewServer">Bool indicating whether the server was already registered in the database or not.</param>
    /// <returns><see langword="true"/> if the timestamp is updated; otherwise <see langword="false"/>.</returns>
    bool UpdateTimestamp(DateTime updateTimestamp, out bool isNewServer);

    /// <summary>
    /// Check for any outdated servers and remove them.
    /// Each server timestamp is compared against the given <paramref name="outdatedTimestamp"/> and removed if the
    /// timestamp is smaller then <paramref name="outdatedTimestamp"/>.
    /// </summary>
    /// <param name="outdatedTimestamp">The <see cref="DateTime"/> to compare against.</param>
    /// <returns><see langword="true"/> if there are outdated servers removed; otherwise <see langword="false"/>.</returns>
    bool CheckForAndRemoveOutdatedServers(DateTime outdatedTimestamp);

    /// <summary>
    /// Get all the registered server names.
    /// </summary>
    /// <returns>Returns a collection with all the registered server names.</returns>
    IEnumerable<string> GetRegisteredServerNames();

    /// <summary>
    /// Reload the Nats server configuration.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    /// <returns><see langword="true"/> if reloading succeeded; otherwise <see langword="false"/>.</returns>
    bool ReloadConfiguration(NatsOptions natsOptions);
}