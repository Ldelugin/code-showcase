using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ITimer = -- redacted --;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="INatsServerOptionsManager"/>.
/// </summary>
public sealed class NatsServerOptionsManager : INatsServerOptionsManager
{
    private const string NatsServerExe = "nats-server.exe";
    private const string ReloadSignal = "--signal reload=Nats";
    private readonly IOptionsMonitor<NatsServerOptions> natsServerOptionsMonitor;
    private readonly IOptionsMonitor<NatsOptions> natsOptionsMonitor;
    private readonly INatsConnectionOptions connectionOptions;
    private readonly IEfSystemConfigurationProvider systemConfigurationProvider;
    private readonly IConfiguration configuration;
    private readonly ILogger<NatsServerOptionsManager> logger;
    private readonly ITimer timer;
    private readonly IDisposable onChangeToken;
    private readonly SemaphoreSlim semaphoreSlim;
    private readonly string serverName = Environment.MachineName;
    private readonly string baseKey;
    private bool isDisposed;

    /// <summary>
    /// Event to report that the NatsOptions is changed.
    /// </summary>
    public event EventHandler<NatsOptionsEventArgs> OptionsChanged;

    /// <summary>
    /// Creates a new instance of <see cref="NatsServerOptionsManager"/>.
    /// </summary>
    /// <param name="natsServerOptionsMonitor">Instance of <see cref="IOptionsMonitor{NatsServerOptions}"/>.</param>
    /// <param name="natsOptionsMonitor">Instance of <see cref="IOptionsMonitor{NatsOptions}"/>.</param>
    /// <param name="connectionOptions">Instance of <see cref="INatsConnectionOptions"/>.</param>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    /// <param name="logger">Instance of <see cref="ILogger{NatsServerOptionsManager}"/>.</param>
    /// <param name="timerFactory">Instance of <see cref="ITimerFactory"/>.</param>
    /// <exception cref="ArgumentNullException">Is thrown when one of the given arguments is null.</exception>
    public NatsServerOptionsManager(IOptionsMonitor<NatsServerOptions> natsServerOptionsMonitor,
        IOptionsMonitor<NatsOptions> natsOptionsMonitor,
        INatsConnectionOptions connectionOptions,
        IConfiguration configuration,
        ILogger<NatsServerOptionsManager> logger,
        ITimerFactory timerFactory)
    {
        if (timerFactory == null)
        {
            throw new ArgumentNullException(nameof(timerFactory));
        }

        this.natsServerOptionsMonitor = natsServerOptionsMonitor ?? throw new ArgumentNullException(nameof(natsServerOptionsMonitor));
        this.natsOptionsMonitor = natsOptionsMonitor ?? throw new ArgumentNullException(nameof(natsOptionsMonitor));
        this.connectionOptions = connectionOptions ?? throw new ArgumentNullException(nameof(connectionOptions));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.baseKey = ConfigurationPath.Combine(-- redacted --, NatsOptions.Name, NatsServerOptions.Name);
        this.systemConfigurationProvider = configuration.GetIEfSystemConfigurationProvider();

        this.semaphoreSlim = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        this.timer = timerFactory.CreateTimer();
        this.timer.Elapsed += this.OnTimerElapsed;
        _ = this.timer.Change(TimeSpan.Zero, this.natsOptionsMonitor.CurrentValue.ConnectionPollingInterval);
        this.onChangeToken = this.natsOptionsMonitor.OnChange(this.OnOptionsChanged);
        this.Save(ServerAndTimestamp.Now(this.serverName)); // Initial save
    }

    /// <summary>
    /// Disposes the connections, timer, onChangeToken and the events.
    /// </summary>
    public void Dispose() => this.Dispose(disposing: true);

    /// <summary>
    /// Update the timestamp for the server if the timestamp of the server is smaller then the
    /// given <paramref name="updateTimestamp"/>.
    /// </summary>
    /// <param name="updateTimestamp">The <see cref="DateTime"/> to compare against.</param>
    /// <param name="isNewServer">Bool indicating whether the server was already registered in the database or not.</param>
    /// <returns><see langword="true"/> if the timestamp is updated; otherwise <see langword="false"/>.</returns>
    public bool UpdateTimestamp(DateTime updateTimestamp, out bool isNewServer)
    {
        var natsServerOptions = this.GetServerOptions(this.serverName, bindToConfiguration: false); // no need to bind.
        // No need to check for null as Get never returns a null object.
        var serverAndTimestamp = new ServerAndTimestamp(this.serverName, natsServerOptions.Timestamp);

        // In case this is a new server, the timestamp will be the default value of 01/01/0001 00:00:00.
        isNewServer = serverAndTimestamp.Timestamp == default;

        if (serverAndTimestamp.Timestamp > updateTimestamp)
        {
            return false;
        }

        // The timestamp from options needs to be updated in/added to the database.
        serverAndTimestamp.UpdateTimestamp(DateTime.UtcNow);
        this.Save(serverAndTimestamp);
        return true;
    }

    /// <summary>
    /// Check for any outdated servers and remove them.
    /// Each server timestamp is compared against the given <paramref name="outdatedTimestamp"/> and removed if the
    /// timestamp is smaller then <paramref name="outdatedTimestamp"/>.
    /// </summary>
    /// <param name="outdatedTimestamp">The <see cref="DateTime"/> to compare against.</param>
    /// <returns><see langword="true"/> if there are outdated servers removed; otherwise <see langword="false"/>.</returns>
    public bool CheckForAndRemoveOutdatedServers(DateTime outdatedTimestamp)
    {
        var hasRemovedAny = false;
        foreach (var registeredServerName in this.GetRegisteredServerNames())
        {
            var natsServerOptions = this.GetServerOptions(registeredServerName);
            var serverAndTimestamp = new ServerAndTimestamp(registeredServerName, natsServerOptions.Timestamp);

            if (serverAndTimestamp.Timestamp > outdatedTimestamp)
            {
                continue;
            }

            // The timestamp from the options is therefore outdated, delete the configuration from the database.
            this.Delete(serverAndTimestamp);
            hasRemovedAny = true;
        }

        return hasRemovedAny;
    }

    /// <summary>
    /// Get all the registered server names.
    /// </summary>
    /// <returns>Returns a collection with all the registered server names.</returns>
    public IEnumerable<string> GetRegisteredServerNames()
    {
        return this.configuration
            .GetSection(this.baseKey)
            ?.GetChildren()
            ?.Select(child => child.Key)
            .ToArray() ?? [];
    }

    /// <summary>
    /// Reload the Nats server configuration.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    /// <returns><see langword="true"/> if reloading succeeded; otherwise <see langword="false"/>.</returns>
    public bool ReloadConfiguration(NatsOptions natsOptions)
    {
        Process process = default;
        var reloaded = true;
        try
        {
            var pathToNatsServer = Path.Combine(natsOptions.NatsServerRootDirectory, NatsServerExe);
            this.logger.OnlyInDebug(l => l.LogDebug("Signal Nats to reload the configuration file"));
            process = new Process();
            process.StartInfo.FileName = pathToNatsServer;
            process.StartInfo.Arguments = ReloadSignal;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            _ = process.Start();

            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                throw new -- redacted --Exception(error);
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Could not reload nats server configuration");
            reloaded = false;
        }
        finally
        {
            process?.Dispose();
        }

        return reloaded;
    }

    /// <summary>
    /// Disposes the timer and onChangeToken.
    /// </summary>
    /// <param name="disposing">Whether this should dispose or not.</param>
    private void Dispose(bool disposing)
    {
        if (!disposing || this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        _ = this.timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        this.timer.Elapsed -= this.OnTimerElapsed;
        this.timer.Dispose();
        this.onChangeToken?.Dispose();
        this.semaphoreSlim.Dispose();
    }

    /// <summary>
    /// Compare the NatsOptions against the NatsConnectionOptions and determine whether there is a change or not.
    /// </summary>
    /// <returns><see langword="true"/> if there is a change; otherwise <see langword="false"/>.</returns>
    private bool IsOptionsChanged()
    {
        var natsOptions = this.natsOptionsMonitor.CurrentValue;
        if (this.IsCredentialsChanged(natsOptions))
        {
            return true;
        }

        if (this.IsPortChanged(natsOptions))
        {
            return true;
        }

        var logMethodChanged = this.connectionOptions.LogDebug != natsOptions.LogDebug ||
                               this.connectionOptions.LogTrace != natsOptions.LogTrace;
        return logMethodChanged || this.IsServersListChanged(natsOptions);
    }

    /// <summary>
    /// Checks whether the credentials changed.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    /// <returns>Returns a <see langword="true"/> is the credentials changed; otherwise <see langword="false"/>.</returns>
    private bool IsCredentialsChanged(NatsOptions natsOptions) =>
        this.connectionOptions.User != natsOptions.NatsServerClientUserName ||
        this.connectionOptions.Password != natsOptions.NatsServerClientPassword;

    /// <summary>
    /// Checks whether the listen port changed.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    /// <returns>Returns a <see langword="true"/> is the listen port changed; otherwise <see langword="false"/>.</returns>
    private bool IsPortChanged(NatsOptions natsOptions) =>
        this.connectionOptions.Port != natsOptions.NatsServerPort;

    /// <summary>
    /// Checks whether the list with server names changed.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    /// <returns>
    /// Returns a <see langword="true"/> is the list with server names changed; otherwise <see langword="false"/>.
    /// </returns>
    private bool IsServersListChanged(NatsOptions natsOptions)
    {
        var serversInPreferredOrder = this.GetRegisteredServerNamesInPreferredOrder(natsOptions.NatsServerPort);

        // If Servers is null and serversInPreferredOrder has any elements then we know that it's changed
        // The null check is to avoid throwing checking the sequence for equality, serversInPreferredOrder is never null.
        if (this.connectionOptions.Servers == null)
        {
            return serversInPreferredOrder.Any();
        }

        return !this.connectionOptions.Servers.SequenceEqual(serversInPreferredOrder);
    }

    /// <summary>
    /// Callback that is raised when the options are changed.
    /// </summary>
    /// <param name="natsOptions">Instance of <see cref="NatsOptions"/>.</param>
    private void OnOptionsChanged(NatsOptions natsOptions) => this.SyncOptionsAndNotifyOptionsChanged();

    /// <summary>
    /// Event callback when the timer is elapsed. 
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The <see cref="TimerEventArgs"/> args containing the timer information.</param>
    private void OnTimerElapsed(object sender, TimerEventArgs args)
    {
        if (!this.IsOptionsChanged())
        {
            return;
        }

        this.SyncOptionsAndNotifyOptionsChanged();
    }

    /// <summary>
    /// Sync the NatsOptions and NatsConnectionOptions and raise the <see cref="OptionsChanged"/> event.
    /// </summary>
    private void SyncOptionsAndNotifyOptionsChanged()
    {
        this.semaphoreSlim.WithLock(() =>
        {
            this.SyncOptions(out var isNewConnectionNeeded);
            this.OptionsChanged?.Invoke(this, new NatsOptionsEventArgs(this.natsOptionsMonitor.CurrentValue,
                isNewConnectionNeeded));
        });
    }

    /// <summary>
    /// Sync the NatsOptions and NatsConnectionOptions.
    /// </summary>
    /// <param name="isNewConnectionNeeded">Indicates whether a new connection is needed.</param>
    private void SyncOptions(out bool isNewConnectionNeeded)
    {
        var natsOptions = this.natsOptionsMonitor.CurrentValue;
        // Before syncing the options, evaluate if any options changed that needs to trigger a new connection.
        isNewConnectionNeeded = this.IsCredentialsChanged(natsOptions) ||
                                this.IsPortChanged(natsOptions) ||
                                this.IsServersListChanged(natsOptions);

        this.connectionOptions.User = natsOptions.NatsServerClientUserName;
        this.connectionOptions.Password = natsOptions.NatsServerClientPassword;
        this.connectionOptions.Port = natsOptions.NatsServerPort;
        this.connectionOptions.LogDebug = natsOptions.LogDebug;
        this.connectionOptions.LogTrace = natsOptions.LogTrace;
        this.connectionOptions.Servers = this.GetRegisteredServerNamesInPreferredOrder(natsOptions.NatsServerPort);
    }

    /// <summary>
    /// Get all the registered server names with the <see cref="serverName"/> first.
    /// </summary>
    /// <param name="port">The port where the server is listening on.</param>
    /// <returns>An array with all the server names and with the <see cref="serverName"/> first.</returns>
    private string[] GetRegisteredServerNamesInPreferredOrder(int port)
    {
        return [.. this.GetRegisteredServerNames()
            .Select(server => $"{NatsOptions.ListenUrlPrefix}{server}:{port}")
            .OrderByDescending(server => server.Contains(this.serverName))
            .ThenBy(server => server)];
    }

    /// <summary>
    /// Get a named instance of <see cref="NatsServerOptions"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="bindToConfiguration"/> is needed when getting the <see cref="NatsServerOptions"/> of a
    /// different server then this method is called from.
    /// So if Server1 and Server2 are registered and Server1 is calling this method, the bind is needed to properly
    /// set the properties for Server2 as this is unknown to Server1.
    /// And thus passing <see langword="true"/> is needed for the <see cref="CheckForAndRemoveOutdatedServers"/> method.
    /// </remarks>
    /// <param name="server">The name of the server.</param>
    /// <param name="bindToConfiguration">
    /// Bind the instance to the configuration which populates all the properties of that instance
    /// loaded from the configuration.
    /// </param>
    /// <returns>An instance of <see cref="NatsServerOptions"/>.</returns>
    private NatsServerOptions GetServerOptions(string server, bool bindToConfiguration = true)
    {
        var natsServerOptions = this.natsServerOptionsMonitor.Get(server);
        if (bindToConfiguration)
        {
            this.configuration.Bind(this.ConstructKey(server), natsServerOptions);
        }
        return natsServerOptions;
    }

    /// <summary>
    /// Delete the configuration in the database for the server given with <paramref name="serverAndTimestamp"/>.
    /// </summary>
    /// <param name="serverAndTimestamp">Instance of <see cref="ServerAndTimestamp"/>.</param>
    private void Delete(ServerAndTimestamp serverAndTimestamp)
    {
        var key = this.ConstructKey(serverAndTimestamp.Server, nameof(NatsServerOptions.Timestamp));
        try
        {
            this.logger.LogInformation("Delete configuration with key {Key}", key);
            this.systemConfigurationProvider.Delete(key);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Unable to delete configuration with key {Key}", key);
        }
    }

    /// <summary>
    /// Save the timestamp for the given server.
    /// </summary>
    /// <param name="serverAndTimestamp">Instance of <see cref="ServerAndTimestamp"/>.</param>
    private void Save(ServerAndTimestamp serverAndTimestamp)
    {
        var key = this.ConstructKey(serverAndTimestamp.Server, nameof(NatsServerOptions.Timestamp));
        try
        {
            this.logger.OnlyInDebug(l => l.LogDebug(
                "Save the configuration with key {Key} and value {Value}",
                key, serverAndTimestamp.TimestampAsString));
            this.systemConfigurationProvider.Set(key, serverAndTimestamp.TimestampAsString);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Can't save the configuration with key {Key}", key);
        }
    }

    /// <summary>
    /// Construct the configuration key.
    /// </summary>
    /// <param name="server">The name of the server.</param>
    /// <param name="propertyName">Optional property name.</param>
    /// <returns>The configuration key separated by the ':' character.</returns>
    private string ConstructKey(string server, string propertyName = null)
        => string.IsNullOrWhiteSpace(propertyName)
            ? ConfigurationPath.Combine(this.baseKey, server)
            : ConfigurationPath.Combine(this.baseKey, server, propertyName);
}