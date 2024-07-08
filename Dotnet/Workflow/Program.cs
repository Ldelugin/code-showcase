using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Workflow;

var parser = new CommandLineBuilder(new Root())
    .UseDefaults()
    .AddMiddleware(async (context, next) =>
    {
        if (context.ParseResult.Tokens.Count == 0)
        {
            context.HelpBuilder.Write(new Root(), Console.Out);
        }
        else
        {
            await next(context);
        }
    })
    .Build();
return await parser.InvokeAsync(args);