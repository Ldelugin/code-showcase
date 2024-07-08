using System.CommandLine;
using Spectre.Console;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;

namespace Workflow.Weekly.Commands;

public class Done : TaskCommand<DoneOptions, DoneOptionsBinder>
{
    private readonly Argument<int> taskIdArgument = new (name: "id", description: "The id of the task to mark as done");
    private readonly Option<int?> subTaskOption = new("--as-sub-task", "Add as sub task for the number provided");
    
    public Done(ITaskRepository taskRepository) : base("done", taskRepository, "Mark a task as done")
    {
        this.AddArgument(this.taskIdArgument);
        this.AddOption(this.subTaskOption);
    }
    
    protected override DoneOptionsBinder GetOptionsBinder() =>
        new(this.weekOption, this.taskIdArgument, this.subTaskOption);
    protected override async Task HandleAsync(DoneOptions options)
    {
        await this.taskRepository.MarkTaskAsDoneAsync(options.Id, options.Week);
        AnsiConsole.MarkupLine($"[green]Task {options.Id} marked as done[/]");
    }
}