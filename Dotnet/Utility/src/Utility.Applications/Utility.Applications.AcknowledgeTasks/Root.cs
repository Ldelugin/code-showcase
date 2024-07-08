using System.CommandLine;
using Utility.Libraries.ServiceReferences;
using Utility.Libraries.ServiceReferences.ManualReviewService;
using Utility.Libraries.Shared.Binders;
using Utility.Libraries.Shared.Options;
using Credentials = Utility.Libraries.Shared.Credentials;

namespace Utility.Applications.AcknowledgeTasks;

public class Root : RootCommand
{
    private CredentialsOptions CredentialIdentifier { get; } = new();
    private Argument<string> TaskId { get; } = new("task-id", () =>
    {
        var line = Console.In.ReadLine();
        var parts = line?.Split(separator: ' ');
        return parts?[0] ?? string.Empty;
    }, "The task ID to acknowledge");
    private Option<StatusEnumeration> Status { get; } = new("--status", () => StatusEnumeration.OK,"The status to set for the task");
    
    public Root() : base("Acknowledge Tasks")
    {
        this.AddArgument(TaskId);
        this.AddOption(CredentialIdentifier);
        this.AddOption(Status);
        this.SetHandler(async (id, credentials, status) =>
        {
            await AcknowledgeTasks(id, credentials, status);
        }, TaskId, new CredentialsBinder(this.CredentialIdentifier), Status);
    }

    private static async Task AcknowledgeTasks(string taskId, Credentials credentials, StatusEnumeration status)
    {
        await using var connector = credentials.CreateConnector();
        await connector.ConnectAsync();
        var acknowledgements = new TaskAcknowledgementType[]
        {
            new()
            {
                TaskID = taskId,
                Value = status,
            }
        };
        var response = await connector.ManualReviewService?.AcknowledgeTasksAsync(acknowledgements)!;
        if (response?.AcknowledgementResult == null)
        {
            return;
        }

        foreach (var acknowledgement in response.AcknowledgementResult)
        {
            await Console.Out.WriteLineAsync($"{acknowledgement.TaskID} {acknowledgement.Value} {acknowledgement.Description}");
        }
    }
}