namespace Workflow.Weekly.Repositories.Models;

public struct TaskFilter
{
    public bool? Done { get; set; }
    public bool? Planned { get; set; }
    public bool? Deleted { get; set; }
    public int Week { get; set; }
    
    public static TaskFilter IncludeDefault() => new()
    {
        Done = true,
        Planned = true,
        Deleted = false
    };
}