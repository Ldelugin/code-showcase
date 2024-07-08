using System.CommandLine.Parsing;
using Workflow.Weekly.Options;

namespace Workflow.Weekly.Binders;

public class ListOptionsBinder : TaskOptionsBinderBase<ListOptions>
{
    private readonly ShowDoneOption showDoneOption;
    private readonly ShowPlannedOption showPlannedOption;
    private readonly ShowDeletedOption showDeletedOption;
    
    public ListOptionsBinder(WeekOption weekOption, ShowDoneOption showDoneOption,
        ShowPlannedOption showPlannedOption, ShowDeletedOption showDeletedOption) : base(weekOption)
    {
        this.showDoneOption = showDoneOption;
        this.showPlannedOption = showPlannedOption;
        this.showDeletedOption = showDeletedOption;
    }
    
    protected override void Parse(ListOptions options, ParseResult parseResult)
    {
        options.Done = parseResult.GetValueForOption(this.showDoneOption);
        options.Planned = parseResult.GetValueForOption(this.showPlannedOption);
        options.Deleted = parseResult.GetValueForOption(this.showDeletedOption);
    }
}