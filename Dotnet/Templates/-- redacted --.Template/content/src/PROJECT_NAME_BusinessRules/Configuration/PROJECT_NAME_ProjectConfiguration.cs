using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
#if (-- redacted --)
using -- redacted --;
#endif
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Options;
using PROJECT_NAME_BusinessRules.BusinessLogic;
using PROJECT_NAME_BusinessRules.Engines;

namespace PROJECT_NAME_BusinessRules.Configuration;

public class PROJECT_NAME_ProjectConfiguration : ProjectConfiguration
{
    public IOptionsMonitor<PROJECT_NAME_BusinessRulesOptions> PROJECT_NAME_BusinessRulesOptions { get; }
    public -- redacted -- { get; }
    public -- redacted -- { get; }
    public -- redacted -- { get; }

    public override IValueValidationRules ValueValidationRules =>
        new PROJECT_NAME_ValueValidationRules();
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override int -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;
    public override bool -- redacted -- =>
        this.PROJECT_NAME_BusinessRulesOptions.CurrentValue.-- redacted --;

    public PROJECT_NAME_ProjectConfiguration(
        -- redacted --,
        IOptionsMonitor<PROJECT_NAME_BusinessRulesOptions> optionsMonitor,
        -- redacted --,
        -- redacted --,
        -- redacted --)
        : base(-- redacted --, optionsMonitor)
    {
        this.-- redacted -- = -- redacted -- ?? throw new ArgumentNullException(nameof(-- redacted --));
        this.-- redacted -- = -- redacted -- ?? throw new ArgumentNullException(nameof(-- redacted --));
        this.-- redacted -- = -- redacted -- ?? throw new ArgumentNullException(nameof(-- redacted --));
        this.PROJECT_NAME_BusinessRulesOptions = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        this.ConfigureProjectConfiguration();
    }

    private void ConfigureProjectConfiguration()
    {
        this.-- redacted -- = new PROJECT_NAME_-- redacted --(this);

#if (-- redacted --)
#if (-- redacted --)
        var -- redacted -- = new List<string> { "-- redacted --" };
        var -- redacted -- = new -- redacted --(
            -- redacted --.ToArray(),
#if (-- redacted --)
            -- redacted --: true,
#else
            -- redacted --: false,
#endif
            this.PROJECT_NAME_BusinessRulesOptions,
            -- redacted --: "-- redacted --",
#if (-- redacted --)
            -- redacted --: true,
#else
            -- redacted --: false,
#endif
#if (-- redacted --)
            -- redacted --: true
#else
            -- redacted --: false
#endif
        );
#endif
#if (-- redacted --)
        var -- redacted -- = new -- redacted --(-- redacted --: "-- redacted --");
#endif
#if (-- redacted --)
        var -- redacted -- = new -- redacted --(
            -- redacted --: "-- redacted --",
            -- redacted --: "-- redacted --",
            -- redacted --: "-- redacted --"
        );
#endif
#if (-- redacted --)
        var -- redacted -- = new -- redacted --(
            -- redacted --: "-- redacted --",
            -- redacted --: "-- redacted --",
            -- redacted --: "-- redacted --"
        );
#endif
#if (-- redacted --)
        var -- redacted -- = new PROJECT_NAME_-- redacted --();
#endif
#endif

        this.-- redacted -- = new -- redacted --
        {
            #if (-- redacted --)
            -- redacted -- = -- redacted --,
            #endif
        };

        this.-- redacted -- = new -- redacted --
        {

        };

        this.-- redacted -- = new -- redacted --
        {

        };

        this.-- redacted -- = new -- redacted --
        {
            
        };
    }
}