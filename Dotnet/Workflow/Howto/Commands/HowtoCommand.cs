using System.CommandLine;

namespace Workflow.Howto.Commands;

public class HowtoCommand : Command
{
    public HowtoCommand() : base("howto", "Howto guides")
    {
    }
}