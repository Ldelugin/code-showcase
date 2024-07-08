namespace Workflow.Weekly.Options;

public class DeleteOptions : TaskOptionsBase
{
    public int Id { get; set; }
    public int? SubTask { get; set; }
}