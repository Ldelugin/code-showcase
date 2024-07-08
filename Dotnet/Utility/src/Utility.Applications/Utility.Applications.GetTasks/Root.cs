using System.CommandLine;
using Utility.Libraries.ServiceReferences;
using Utility.Libraries.Shared.Binders;
using Utility.Libraries.Shared.Options;
using Credentials = Utility.Libraries.Shared.Credentials;

namespace Utility.Applications.GetTasks;

public class Root : RootCommand
{
    private CredentialsOptions CredentialIdentifier { get; } = new();
    private Option<string> Queue { get; } = new("--queue", () => Constants.DefaultQueue,"The queue to get tasks from");
    private Option<int> MaxTasks { get; } = new("--max-tasks", () => Constants.DefaultMaxTasks, "The maximum number of tasks to get");
    private Option<bool> IdOnly { get; } = new("--id-only", () => false ,"Only show the task ID");

    public Root() : base("Get Tasks")
    {
        this.AddOption(CredentialIdentifier);
        this.AddOption(Queue);
        this.AddOption(MaxTasks);
        this.AddOption(IdOnly);
        this.SetHandler(async (credentials, queue, maxTasks, idOnly) =>
        {
            await GetTasks(credentials, queue, maxTasks, idOnly);
        }, new CredentialsBinder(this.CredentialIdentifier), Queue, MaxTasks, IdOnly);
    }

    private static async Task GetTasks(Credentials credentials, string queue, int maxTasks, bool idOnly)
    {
        await using var connector = new Connector(credentials.Url, credentials.Username, credentials.Password);
        await connector.ConnectAsync();
        var response = await connector.ManualReviewService?.GetTasksAsync(queue, maxTasks)!;
        if (response?.Task == null)
        {
            return;
        }

        foreach (var task in response.Task)
        {
            if (idOnly)
            {
                await Console.Out.WriteLineAsync(task.TaskID);
                continue;
            }

            await Console.Out.WriteLineAsync($"{task.TaskID} {task.Type} {task.DataStream} {task.CreationTimeUTC}");
        }
    }
}