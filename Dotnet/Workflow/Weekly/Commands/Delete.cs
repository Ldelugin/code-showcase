using System.CommandLine;
using Spectre.Console;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;

namespace Workflow.Weekly.Commands;

public class Delete : TaskCommand<DeleteOptions, DeleteOptionsBinder>
{
    private readonly Argument<int> taskIdArgument = new (name: "id", description: "The id of the task to delete");
    private readonly Option<int?> subTaskOption = new("--as-sub-task", "Add as sub task for the number provided");
    
    public Delete(ITaskRepository taskRepository) : base("delete", taskRepository, "delete a task or sub task")
    {
        this.AddArgument(this.taskIdArgument);
        this.AddOption(this.subTaskOption);
    }
    
    protected override DeleteOptionsBinder GetOptionsBinder() =>
        new(this.weekOption, this.taskIdArgument, this.subTaskOption);

    protected override async Task HandleAsync(DeleteOptions options)
    {
        await this.taskRepository.DeleteTaskAsync(options.Id, options.Week);
        AnsiConsole.MarkupLine($"[green]Task {options.Id} deleted[/]");
    }
}