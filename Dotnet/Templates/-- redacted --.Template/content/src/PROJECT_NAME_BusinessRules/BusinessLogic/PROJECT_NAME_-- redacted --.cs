using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using PROJECT_NAME_BusinessRules.Configuration;

namespace PROJECT_NAME_BusinessRules.BusinessLogic;

public class PROJECT_NAME_-- redacted -- : -- redacted --
{
    private readonly PROJECT_NAME_ProjectConfiguration configuration;
    
    public PROJECT_NAME_-- redacted --(PROJECT_NAME_ProjectConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public IEnumerable<-- redacted --> StepDescriptors => new List<-- redacted -->
    {
        // TODO: Implement me!
    };
    
    public -- redacted -- TryToIdentify(ICollection<-- redacted --> -- redacted --, -- redacted --)
    {
        // TODO: Implement me!
        return null;
    }
    
    -- redacted --
}