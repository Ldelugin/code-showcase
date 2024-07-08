using System.CommandLine;

namespace Workflow.Weekly.Options;

public class ShowDoneOption : Option<bool?>
{
    public ShowDoneOption() : base("--show-done", "Show tasks that are done")
    {
        this.SetDefaultValue(false);
    }
}