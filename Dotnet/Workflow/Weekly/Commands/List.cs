using Spectre.Console;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Entities;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;
using Workflow.Weekly.Repositories.Models;

namespace Workflow.Weekly.Commands;

public class List : TaskCommand<ListOptions, ListOptionsBinder>
{
    private readonly ShowDoneOption showDoneOption = new();
    private readonly ShowPlannedOption showPlannedOption = new();
    private readonly ShowDeletedOption showDeletedOption = new();
    
    public List(ITaskRepository taskRepository) : base("list", taskRepository, "List tasks")
    {
        this.AddOption(this.showDoneOption);
        this.AddOption(this.showPlannedOption);
        this.AddOption(this.showDeletedOption);
    }

    protected override ListOptionsBinder GetOptionsBinder() => new(this.weekOption, this.showDoneOption,
        this.showPlannedOption, this.showDeletedOption);

    protected override async Task HandleAsync(ListOptions options)
    {
        var filter = new TaskFilter
        {
            Week = options.Week,
            Deleted = options.Deleted,
            Done = options.Done,
            Planned = options.Planned
        };

        var result = await this.taskRepository.GetTasksAsync(filter);
        var tasks = result.OrderBy(task => task.IsDone).ToArray();
        
        AnsiConsole.MarkupLine($"Week [bold]{options.Week}[/]");
        AnsiConsole.WriteLine("Planned:");
        PrintTasks(tasks.Where(task => task.IsPlanned));
        
        AnsiConsole.WriteLine("Not Planned:");
        PrintTasks(tasks.Where(task => !task.IsPlanned));
    }

    private static void PrintTasks(IEnumerable<TaskItem> tasks)
    {
        foreach (var task in tasks)
        {
            PrintTask(task);
            foreach (var subTask in task.SubTasks.OrderBy(t => task.IsDone))
            {
                PrintTask(subTask, indent: 2);
            }
        }
    }

    private static void PrintTask(TaskBase taskItem, int indent = 1)
    {
        var description = taskItem.Description;
        var id = taskItem.Id;
        var indentString = new string(' ', indent * 2);
        var markup = taskItem.IsDone ? "strikethrough" : Color.Default.ToString();
        var prefix = taskItem.IsDone ? "[*]" : "[ ]";
        AnsiConsole.MarkupLineInterpolated($"{indentString}{id}. {prefix} [{markup}]{description}[/]");
    }
}