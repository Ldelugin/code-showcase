using System.CommandLine;
using System.Text;
using Spectre.Console;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Entities;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;
using Workflow.Weekly.Repositories.Models;

namespace Workflow.Weekly.Commands;

public class Show : TaskCommand<ShowOptions, ShowOptionsBinder>
{
    private readonly Option<int?> taskIdOption = new("--id", "Show only the task with the provided id");
    private readonly ShowDeletedOption showDeletedOption = new();
    
    public Show(ITaskRepository taskRepository) : base("show", taskRepository, "Show the current week")
    {
        this.AddOption(this.taskIdOption);
        this.AddOption(this.showDeletedOption);
    }
    
    protected override ShowOptionsBinder GetOptionsBinder() =>
        new(this.weekOption, this.taskIdOption, this.showDeletedOption);
    protected override async Task HandleAsync(ShowOptions options)
    {
        var builder = new StringBuilder();
        Console.WriteLine($"Id: {options.Id}");
        
        if (options.Id is null or 0)
        {
            await this.ShowAll(options, builder);
        } else
        {
            await this.ShowTask(options, builder);
        }

        AnsiConsole.WriteLine(builder.ToString());
    }

    private async Task ShowAll(ShowOptions options, StringBuilder builder)
    {
        builder.Append("Week:").Append(options.Week).AppendLine().AppendLine();
        
        var filter = new TaskFilter
        {
            Week = options.Week,
            Deleted = options.Deleted
        };

        var result = await this.taskRepository.GetTasksAsync(filter);
        var tasks = result.ToArray();

        foreach (var task in tasks)
        {
            PrintTask(task, builder);
            foreach (var subTask in task.SubTasks)
            {
                PrintTask(subTask, builder, indent: 2);
            }
        }
    }
    
    private async Task ShowTask(ShowOptions options, StringBuilder builder)
    {
        var taskItem = await this.taskRepository.GetTaskAsync(options.Id!.Value, options.Week);
        PrintTask(taskItem, builder, options.Week);
    }
    
    private static void PrintTask(TaskItem task, StringBuilder builder, int week, int indent = 1)
    {
        builder.AppendLine(task.Description);
        builder.Append(new string(' ', indent * 2)).Append("Week: ").Append(week).AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Id: ").Append(task.Id).AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Created: ").Append(task.CreatedAt).AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Done: ").Append(task.IsDone);
        
        if (task.DoneAt.HasValue)
        {
            builder.Append(" at ").Append(task.DoneAt.Value);
        }
        builder.AppendLine();
        
        builder.Append(new string(' ', indent * 2)).Append("Planned: ").Append(task.IsPlanned).AppendLine();

        builder.Append(new string(' ', indent * 2)).Append("Deleted: ").Append(task.IsDone);

        if (task.DeletedAt.HasValue)
        {
            builder.Append(" at ").Append(task.DeletedAt.Value);
        }
        
        builder.AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Sub tasks: ").Append(task.SubTasks.Count).AppendLine();
        foreach (var subTask in task.SubTasks)
        {
            PrintSubTask(subTask, builder, indent: 2);
        }
    }
    
    private static void PrintSubTask(SubTaskItem task, StringBuilder builder, int indent = 1)
    {
        builder.Append(new string(' ', indent * 2)).AppendLine(task.Description);
        builder.Append(new string(' ', indent * 2)).Append("Id: ").Append(task.Id).AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Created: ").Append(task.CreatedAt).AppendLine();
        builder.Append(new string(' ', indent * 2)).Append("Done: ").Append(task.IsDone);
        
        if (task.DoneAt.HasValue)
        {
            builder.Append(" at ").Append(task.DoneAt.Value);
        }
        builder.AppendLine();

        builder.Append(new string(' ', indent * 2)).Append("Deleted: ").Append(task.IsDone);

        if (task.DeletedAt.HasValue)
        {
            builder.Append(" at ").Append(task.DeletedAt.Value);
        }
        
        builder.AppendLine();
    }

    private static void PrintTask(TaskBase task, StringBuilder builder, int indent = 1)
    {
        builder.Append(new string(' ', indent * 2)).Append("- ").AppendLine(task.Description);
    }
}