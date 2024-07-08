using System.CommandLine;
using Workflow.Weekly.Binders;
using Workflow.Weekly.Options;
using Workflow.Weekly.Repositories;

namespace Workflow.Weekly.Commands;

public abstract class TaskCommand<TOptions, TOptionsBinder> : Command
    where TOptions : TaskOptionsBase, new()
    where TOptionsBinder : TaskOptionsBinderBase<TOptions>
{
    protected readonly WeekOption weekOption = new();
    protected readonly ITaskRepository taskRepository;
    
    protected TaskCommand(string name, ITaskRepository taskRepository, string? description = null) : base(name, description)
    {
        this.taskRepository = taskRepository;
        this.AddOption(this.weekOption);
        this.SetHandler(this.HandleAsync, this.GetOptionsBinder());
    }

    protected abstract TOptionsBinder GetOptionsBinder();
    protected abstract Task HandleAsync(TOptions options);
}