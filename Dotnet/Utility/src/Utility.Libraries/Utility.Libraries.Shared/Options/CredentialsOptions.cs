using System.CommandLine;

namespace Utility.Libraries.Shared.Options;

public class CredentialsOptions : Option<string>
{
    public CredentialsOptions() : base("--cred-id", "The identifier of the credentials")
    {
        this.AddAlias("-c");
    }
}