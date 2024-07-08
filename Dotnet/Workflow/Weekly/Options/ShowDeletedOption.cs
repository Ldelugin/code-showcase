using System.CommandLine;

namespace Workflow.Weekly.Options;

public class ShowDeletedOption : Option<bool?>
{
    public ShowDeletedOption() : base("--show-deleted", "Show tasks that are deleted")
    {
        this.SetDefaultValue(false);
    }
}