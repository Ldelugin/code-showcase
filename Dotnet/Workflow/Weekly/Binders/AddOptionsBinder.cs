using System.CommandLine;
using System.CommandLine.Parsing;
using Workflow.Weekly.Options;

namespace Workflow.Weekly.Binders;

public class AddOptionsBinder : TaskOptionsBinderBase<AddOptions>
{
    private readonly Argument<string> descriptionArgument;
    private readonly Option<bool> isPlannedOptions;
    private readonly Option<int?> subTaskOption;
    
    public AddOptionsBinder(WeekOption weekOption, Argument<string> descriptionArgument, Option<bool> isPlannedOptions,
        Option<int?> subTaskOption) : base(weekOption)
    {
        this.descriptionArgument = descriptionArgument;
        this.isPlannedOptions = isPlannedOptions;
        this.subTaskOption = subTaskOption;
    }
    
    protected override void Parse(AddOptions options, ParseResult parseResult)
    {
        options.Description = parseResult.GetValueForArgument(this.descriptionArgument);
        options.IsPlanned = parseResult.GetValueForOption(this.isPlannedOptions);
        options.SubTask = parseResult.GetValueForOption(this.subTaskOption);
    }
}