using System.CommandLine;
using Utility.Applications.Credentials.Commands;

namespace Utility.Applications.Credentials;

public class Root : RootCommand
{
    public Root() : base("Credentials")
    {
        this.AddCommand(new Add());
        this.AddCommand(new Get());
    }
}