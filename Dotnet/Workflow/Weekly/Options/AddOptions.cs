namespace Workflow.Weekly.Options;

public class AddOptions : TaskOptionsBase
{
    public string Description { get; set; } = string.Empty;
    public bool IsPlanned { get; set; }
    public int? SubTask { get; set; }
}