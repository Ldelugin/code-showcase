using System.CommandLine;

namespace Utility.Libraries.Shared.Arguments;

public class CredentialsArgument() : Argument<string>("identifier", "The identifier of the credentials")
{
}