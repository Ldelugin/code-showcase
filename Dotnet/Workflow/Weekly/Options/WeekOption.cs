using System.CommandLine;
using Workflow.Extensions;

namespace Workflow.Weekly.Options;

public class WeekOption : Option<int>
{
    public WeekOption() : base("--week", "The week number to use")
    {
        this.SetDefaultValue(DateTime.Now.GetIso8601WeekOfYear());
    }
}