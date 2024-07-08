using System.CommandLine;
using System.CommandLine.Parsing;
using Workflow.Weekly.Options;

namespace Workflow.Weekly.Binders;

public class ShowOptionsBinder : TaskOptionsBinderBase<ShowOptions>
{
    private readonly Option<int?> taskIdOptions;
    private readonly ShowDeletedOption showDeletedOption;
    
    public ShowOptionsBinder(WeekOption weekOption, Option<int?> taskIdOptions, ShowDeletedOption showDeletedOption) : base(weekOption)
    {
        this.taskIdOptions = taskIdOptions;
        this.showDeletedOption = showDeletedOption;
    }
    
    protected override void Parse(ShowOptions options, ParseResult parseResult)
    {
        options.Id = parseResult.GetValueForOption(this.taskIdOptions);
        options.Deleted = parseResult.GetValueForOption(this.showDeletedOption);
    }
}