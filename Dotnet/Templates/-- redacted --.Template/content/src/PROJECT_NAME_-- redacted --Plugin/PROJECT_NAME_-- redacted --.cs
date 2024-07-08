using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PROJECT_NAME_-- redacted --.Configuration;

namespace PROJECT_NAME_-- redacted --;

public class PROJECT_NAME_-- redacted -- : -- redacted --<PROJECT_NAME_ProjectOptions>
{
    public override string ProjectName => "PROJECT_NAME_";
    public override IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration, -- redacted -- plugin)
    {
        _ = services.AddOptions<PROJECT_NAME_ProjectOptions>().Bind(configuration.GetSection(plugin.ConfigurationSectionName));
        
        return services
            .AddApiPlugin<PROJECT_NAME_ApiPlugin>();
    }
}