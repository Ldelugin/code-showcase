using System.CommandLine;
using Utility.Libraries.Shared.Arguments;
using Utility.Libraries.Shared.Binders;

namespace Utility.Applications.Credentials.Commands;

public class Get : Command
{
    private readonly CredentialsArgument identifierArgument = new();

    public Get() : base("get", "Get a credentials")
    {
        this.AddArgument(this.identifierArgument);
        this.SetHandler(Handle, new CredentialsBinder(this.identifierArgument));
    }
    
    private static void Handle(Libraries.Shared.Credentials credentials)
    {
        var url = credentials.Url;
        var username = credentials.Username;
        var password = credentials.Password;
        Console.WriteLine($"Url: {url}");
        Console.WriteLine($"Username: {username}");
        Console.WriteLine($"Password: {password}");
    }
}