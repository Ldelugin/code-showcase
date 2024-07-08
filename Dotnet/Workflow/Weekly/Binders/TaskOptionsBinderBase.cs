using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using Workflow.Weekly.Options;

namespace Workflow.Weekly.Binders;

public abstract class TaskOptionsBinderBase<T> : BinderBase<T>
    where T : TaskOptionsBase, new()
{
    private readonly WeekOption weekOption;

    protected TaskOptionsBinderBase(WeekOption weekOption)
    {
        this.weekOption = weekOption;
    }
    
    protected override T GetBoundValue(BindingContext bindingContext)
    {
        var options = new T
        {
            Week = bindingContext.ParseResult.GetValueForOption(this.weekOption)
        };

        this.Parse(options, bindingContext.ParseResult);
        return options;
    }

    protected abstract void Parse(T options, ParseResult parseResult);
}