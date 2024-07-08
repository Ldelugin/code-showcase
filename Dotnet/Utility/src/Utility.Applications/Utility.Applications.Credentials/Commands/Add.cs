using System.CommandLine;
using Utility.Libraries.Shared.Arguments;
using Utility.Libraries.Shared.Binders;

namespace Utility.Applications.Credentials.Commands;

public class Add : Command
{
    private readonly CredentialsArgument identifierArgument = new();
    private readonly Option<string> urlOption = new("--url");
    private readonly Option<string> usernameOption = new("--username");
    private readonly Option<string> passwordOption = new("--password");
    
    public Add() : base("add", "Add a credentials")
    {
        this.AddArgument(this.identifierArgument);
        this.AddOption(this.urlOption);
        this.AddOption(this.usernameOption);
        this.AddOption(this.passwordOption);
        this.SetHandler(Handle,
            new CredentialsBinder(this.identifierArgument),
            this.urlOption,
            this.usernameOption,
            this.passwordOption);
    }

    private static void Handle(Libraries.Shared.Credentials credentials, string url, string username, string password)
    {
        credentials.Set(url, username, password);
        Console.WriteLine($"Credentials for {credentials.Identifier} added successfully.");
    }
}