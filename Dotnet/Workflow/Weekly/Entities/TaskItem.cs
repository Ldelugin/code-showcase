namespace Workflow.Weekly.Entities;

public class TaskItem : TaskBase
{
    public bool IsPlanned { get; set; }
    public List<SubTaskItem> SubTasks { get; set; } = new();
}