using System.Text;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Logging;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="IMessagePoster"/> to handle posting messages to a NATS server.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="NatsMessagesPoster"/>.
/// </remarks>
/// <param name="natsConnection">Instance of <see cref="INatsConnection"/>.</param>
/// <param name="logger">Instance of <see cref="ILogger{NatsMessagesPoster}"/>.</param>
public sealed class NatsMessagesPoster(INatsConnection natsConnection, ILogger<NatsMessagesPoster> logger) : NatsMessagesBase(natsConnection, logger), IMessagePoster
{
    -- redacted --

    /// <summary>
    /// Post a log message.
    /// </summary>
    /// <param name="value">The log object to post.</param>
    /// <returns>Returns true when the log message is successfully posted, otherwise false is returned.</returns>
    public bool PostLog(Log value) =>
        this.NatsConnection.TryPublish($"{NatsMessagesSubjects.Log}.{value.ServiceName}",
            value.ToByteArray());

    -- redacted --

    /// <summary>
    /// Post a request to reboot a machine.
    /// </summary>
    /// <param name="machineName">The name of the machine to reboot.</param>
    /// <param name="force">Whether the machine should be forcefully rebooted.</param>
    /// <returns>Returns true if the reboot message is successfully posted, otherwise false is returned.</returns>
    public bool PostRebootRequest(string machineName, bool force = false) =>
        this.NatsConnection.TryPublish($"{NatsMessagesSubjects.Reboot}.{machineName}",
            Encoding.UTF8.GetBytes(force.ToString()));

    -- redacted --
}