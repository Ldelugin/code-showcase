namespace Workflow.Feedback.Entities;

public class FeedbackItem
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}