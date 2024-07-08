using System.CommandLine.Binding;
using Utility.Libraries.Shared.Arguments;
using Utility.Libraries.Shared.Options;

namespace Utility.Libraries.Shared.Binders;

public class CredentialsBinder
    : BinderBase<Credentials>
{
    private readonly CredentialsOptions? credentialsOptions;
    private readonly CredentialsArgument? credentialsArgument;
    
    public CredentialsBinder(CredentialsOptions credentialsOptions)
    {
        this.credentialsOptions = credentialsOptions;
    }
    
    public CredentialsBinder(CredentialsArgument credentialsArgument)
    {
        this.credentialsArgument = credentialsArgument;
    }
    
    protected override Credentials GetBoundValue(BindingContext bindingContext)
    {
        var identifier = string.Empty;
        
        if (this.credentialsOptions != null)
        {
            identifier = bindingContext.ParseResult.GetValueForOption(credentialsOptions);
        }
        else if (this.credentialsArgument != null)
        {
            identifier = bindingContext.ParseResult.GetValueForArgument(this.credentialsArgument);
        }

        return new Credentials(identifier);
    }
}