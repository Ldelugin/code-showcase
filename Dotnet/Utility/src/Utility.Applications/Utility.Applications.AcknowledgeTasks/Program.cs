using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Utility.Applications.AcknowledgeTasks;

var parser = new CommandLineBuilder(new Root())
    .UseDefaults()
    .Build();

return await parser.InvokeAsync(args);