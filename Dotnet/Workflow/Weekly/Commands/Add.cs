using System.CommandLine;
using Spectre.Console;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Entities;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;

namespace Workflow.Weekly.Commands;

public class Add : TaskCommand<AddOptions, AddOptionsBinder>
{
    private readonly Argument<string> descriptionArgument = new(name: "description", description: "Description of the task");
    private readonly Option<bool> isPlannedOptions = new("--planned", getDefaultValue: () => true,
        "Indicates that the task is planned");
    private readonly Option<int?> subTaskOption = new("--as-sub-task", "Add as sub task for the number provided");
    
    public Add(ITaskRepository taskRepository) : base("add", taskRepository, "Add a new task")
    {
        this.AddArgument(this.descriptionArgument);
        this.AddOption(this.isPlannedOptions);
        this.AddOption(this.subTaskOption);
    }

    protected override AddOptionsBinder GetOptionsBinder() =>
        new(this.weekOption, this.descriptionArgument, this.isPlannedOptions, this.subTaskOption);
    
    protected override async Task HandleAsync(AddOptions options)
    {
        var id = await this.taskRepository.GetNextIdAsync(options.Week);
        
        var task = new TaskItem
        {
            Description = options.Description,
            CreatedAt = DateTime.Now,
            Id = id,
            IsPlanned = options.IsPlanned
        };

        await this.taskRepository.CreateTaskAsync(task, options.Week);
        AnsiConsole.MarkupLine($"[green]Task {id} created[/]");
    }
}