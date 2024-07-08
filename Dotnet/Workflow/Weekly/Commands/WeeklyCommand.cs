using System.CommandLine;
using Workflow.Weekly.Repositories;

namespace Workflow.Weekly.Commands;

public class WeeklyCommand : Command
{
    private readonly ITaskRepository taskRepository;
    
    public WeeklyCommand(ITaskRepository taskRepository) : base("weekly", "Weekly")
    {
        this.taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        this.AddCommand(new Add(this.taskRepository));
        this.AddCommand(new Delete(this.taskRepository));
        this.AddCommand(new Done(this.taskRepository));
        this.AddCommand(new List(this.taskRepository));
        this.AddCommand(new Show(this.taskRepository));
    }
}