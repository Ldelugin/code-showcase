namespace Workflow.Weekly.Options;

public class ListOptions : TaskOptionsBase
{
    public bool? Done { get; set; }
    public bool? Planned { get; set; }
    public bool? Deleted { get; set; }
}