using System;
using System.Linq;
using System.Threading;
using -- redacted --g;
using -- redacted --;
using -- redacted --;
using -- redacted --n;
using LiteDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="ILiveLogService"/>.
/// </summary>
public sealed class LiveLogService : ILiveLogService
{
    /// <summary>
    /// Roughly the average logs per Mb stored, based on a calculation and took a lower amount to be sure.
    /// </summary>
    private const long TotalAverageLogsPerMb = 650;
    private const long OneMbInBytes = 1_048_576;
    private const int PercentageOfTotalLogsToDelete = 10;

    private readonly IMessageReceiver messageReceiver;
    private readonly ILogger<LiveLogService> logger;
    private readonly Timer timer;
    private readonly IOptionsMonitor<-- redacted --> optionsMonitor;
    private LiteDatabase liteDatabase;
    private ILiteCollection<LogModel> logsCollection;

    /// <summary>
    /// Creates a new instance of <see cref="LiveLogService"/>.
    /// </summary>
    /// <param name="messageReceiver">The instance of <see cref="IMessageReceiver"/> to receive incoming log messages.</param>
    /// <param name="logger">Instance of <see cref="ILogger{LiveLogService}"/></param>
    /// <param name="optionsMonitor">Instance of <see cref="IOptionsMonitor{-- redacted --}"/></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided <paramref name="messageReceiver"/> is null or
    /// thrown when the provided <paramref name="logger"/> is null or
    /// thrown when the provided <paramref name="optionsMonitor"/> is null.
    /// </exception>
    public LiveLogService(IMessageReceiver messageReceiver,
        ILogger<LiveLogService> logger,
        IOptionsMonitor<-- redacted --> optionsMonitor)
    {
        this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        this.timer = new Timer(this.TimerCallback, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        this.Start();
    }

    /// <summary>
    /// The due time that is used to wait until it should cleanup the database.
    /// </summary>
    private TimeSpan CleanupDueTime => this.optionsMonitor.CurrentValue.LoggingOptions.LogDbCleanupDueTime;

    /// <summary>
    /// The size limit of the database.
    /// </summary>
    private long DbSizeLimit => Math.Abs(this.optionsMonitor.CurrentValue.LoggingOptions.LogDbSizeLimitInMb) * OneMbInBytes;

    /// <summary>
    /// Get an array of logs that could be found with the provided <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request criteria.</param>
    /// <returns>
    /// Returns any logs that could be found with the request.
    /// </returns>
    public LogModel[] GetLogs(LiveLogViewRequest request)
    {
        if (!this.optionsMonitor.CurrentValue.LoggingOptions.IsLiveLogViewEnabled)
        {
            if (this.logsCollection != null)
            {
                this.Stop();
            }

            return [];
        }

        if (this.logsCollection == null)
        {
            this.Start();
            return [];
        }

        try
        {
            this.ResetTimer();
            var query = this.logsCollection.Query();

            FilterQuery(request, query);

            _ = query.OrderByDescending(x => x.Id);
            _ = query.Limit(request.Limit ?? 100);
            return query.ToArray();
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occured while querying the live log database");
            return [];
        }
    }

    /// <summary>
    /// Filter the given <paramref name="query"/> based on the given <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request criteria.</param>
    /// <param name="query">The query.</param>
    private static void FilterQuery(LiveLogViewRequest request, ILiteQueryable<LogModel> query)
    {
        if (request.LogLevels != null && request.LogLevels.Any())
        {
            _ = query.Where(x => request.LogLevels.Contains(x.LogLevel));
        }

        if (request.ProcessIds != null && request.ProcessIds.Any())
        {
            _ = query.Where(x => request.ProcessIds.Contains(x.ProcessId));
        }

        if (request.ServiceNames != null && request.ServiceNames.Any())
        {
            _ = query.Where(x => request.ServiceNames.Contains(x.ServiceName));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchInMessage))
        {
            _ = query.Where(x => x.Message.Contains(request.SearchInMessage));
        }

        if (!request.StartBucket.HasValue)
        {
            return;
        }

        var now = DateTime.Now;
        var start = now.Date.Add(request.StartBucket.Value);
        var end = request.EndBucket.HasValue ? now.Date.Add(request.EndBucket.Value) : now.AddMinutes(1);
        _ = query.Where(x => x.Timestamp >= start && x.Timestamp <= end);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <footer>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose?view=netframework-4.7.2">`IDisposable.Dispose` on docs.microsoft.com</a>
    /// </footer>
    public void Dispose()
    {
        this.messageReceiver?.Dispose();
        this.timer?.Dispose();
        this.liteDatabase?.Dispose();
    }

    /// <summary>
    /// Raised when the <see cref="IMessageReceiver"/> received a log.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">Instance of <see cref="-- redacted --"/>.</param>
    private void OnLogReceived(object sender, -- redacted -- args)
    {
        if (!this.optionsMonitor.CurrentValue.LoggingOptions.IsLiveLogViewEnabled)
        {
            if (this.logsCollection != null)
            {
                this.Stop();
            }

            return;
        }

        try
        {
            _ = this.logsCollection.Insert((LogModel)Log.FromByteArray(args.Data));
        }
        catch (LiteException e) when (e.Message.Contains("Maximum data file size has been reached"))
        {
            this.DeleteOldestLogsAndRetryLastReceived((LogModel)Log.FromByteArray(args.Data));
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while inserting a live log entry");
        }
    }

    /// <summary>
    /// Delete the oldest logs and retry the last received log.
    /// </summary>
    /// <param name="logModel">The last received log.</param>
    private void DeleteOldestLogsAndRetryLastReceived(LogModel logModel)
    {
        try
        {
            var toDelete = this.CalculateDeleteAmountBasedOnSizeLimit();
            var oldest = this.logsCollection.Query().OrderBy(x => x.Timestamp).First();
            _ = this.logsCollection.DeleteMany(x => x.Id < oldest.Id + toDelete);
            _ = this.logsCollection.Insert(logModel);
        }
        catch (LiteException)
        {
            // ignored to not create a fork bom.
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "An error occurred while deleting a live log entry");
        }
    }

    /// <summary>
    /// Calculate the amount of logs to delete based on the size limit.
    /// </summary>
    /// <returns>The amount to delete.</returns>
    private long CalculateDeleteAmountBasedOnSizeLimit()
    {
        var totalInLogs = this.DbSizeLimit / TotalAverageLogsPerMb;
        return totalInLogs / 100 * PercentageOfTotalLogsToDelete;
    }

    /// <summary>
    /// Reset the timer without triggering the callback.
    /// </summary>
    private void ResetTimer() => this.timer.Change(this.CleanupDueTime, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Start the database and listen to receive logs.
    /// </summary>
    private void Start()
    {
        this.liteDatabase = new LiteDatabase(":memory:")
        {
            LimitSize = this.DbSizeLimit
        };
        this.logsCollection = this.liteDatabase.GetCollection<LogModel>();
        _ = this.logsCollection.EnsureIndex(x => x.Timestamp);
        _ = this.logsCollection.EnsureIndex(x => x.LogLevel);
        _ = this.logsCollection.EnsureIndex(x => x.ProcessId);
        _ = this.logsCollection.EnsureIndex(x => x.ServiceName);
        this.messageReceiver.LogReceived += this.OnLogReceived;
        this.ResetTimer();
        this.logger.LogInformation("Start the database");
    }

    /// <summary>
    /// Stop the database, dispose it and stop listen for logs.
    /// </summary>
    private void Stop()
    {
        this.messageReceiver.LogReceived -= this.OnLogReceived;
        this.logsCollection = null;
        this.liteDatabase?.Dispose();
        this.liteDatabase = null;
        _ = this.timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        this.logger.LogInformation("Stop and disposes of the database");
    }

    /// <summary>
    /// The timer callback, which will call <see cref="Stop"/>.
    /// </summary>
    /// <param name="state">The state as object, is null when nog used.</param>
    private void TimerCallback(object state) => this.Stop();
}
