using System.CommandLine;
using System.CommandLine.Parsing;
using Workflow.Weekly.Options;

namespace Workflow.Weekly.Binders;

public class DeleteOptionsBinder : TaskOptionsBinderBase<DeleteOptions>
{
    private readonly Argument<int> taskIdArgument;
    private readonly Option<int?> subTaskOption;
    
    public DeleteOptionsBinder(WeekOption weekOption, Argument<int> taskIdArgument, Option<int?> subTaskOption)
        : base(weekOption)
    {
        this.taskIdArgument = taskIdArgument;
        this.subTaskOption = subTaskOption;
    }
    
    protected override void Parse(DeleteOptions options, ParseResult parseResult)
    {
        options.Id = parseResult.GetValueForArgument(this.taskIdArgument);
        options.SubTask = parseResult.GetValueForOption(this.subTaskOption);
    }
}