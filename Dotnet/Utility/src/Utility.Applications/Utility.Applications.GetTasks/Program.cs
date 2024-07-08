using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Utility.Applications.GetTasks;

var parser = new CommandLineBuilder(new Root())
    .UseDefaults()
    .Build();

return await parser.InvokeAsync(args);