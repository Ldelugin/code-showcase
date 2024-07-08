using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using System.Reflection;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using PROJECT_NAME_BusinessRules.Configuration;
using PROJECT_NAME_BusinessRules.Engines;

namespace PROJECT_NAME_BusinessRules;

public class PROJECT_NAME_BusinessRules : -- redacted --<PROJECT_NAME_BusinessRulesOptions>
{
    -- redacted --
    
    public override string BrVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
    public override string BrName => "PROJECT_NAME_ Business Rules";
    -- redacted --

    public override IProjectConfiguration ProjectConfiguration
    {
        get => this.projectConfiguration ??= new PROJECT_NAME_ProjectConfiguration(
            -- redacted --);
        protected set => this.projectConfiguration = value;
    }
    
    public PROJECT_NAME_BusinessRules(-- redacted --) : base(-- redacted --)
    {
       -- redacted --
    }
    
    -- redacted --
}