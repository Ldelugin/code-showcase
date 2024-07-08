using System.CommandLine;

namespace Workflow.Weekly.Options;

public class ShowPlannedOption : Option<bool?>
{
    public ShowPlannedOption() : base("--show-planned", "Show tasks that are planned")
    {
        this.SetDefaultValue(true);
    }
}