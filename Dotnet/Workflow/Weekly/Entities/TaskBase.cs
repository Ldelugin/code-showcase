namespace Workflow.Weekly.Entities;

public class TaskBase
{
    public int Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DoneAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}