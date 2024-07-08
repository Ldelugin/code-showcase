using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;

namespace PROJECT_NAME_BusinessRules.Configuration;

public class PROJECT_NAME_-- redacted -- : -- redacted --
{
    private const string PurposeData = "Data Identification";
    private const string PurposeBusinessRules = "Business Rules";
    private const string PurposeStatistics = "Statistics";
    
    public string Name => "PROJECT_NAME_ -- redacted --";
    public string VersionInfo => this.GetType().Assembly.GetName().Version!.ToString();
    public string Description => "-- redacted --";

    public void ApplyConstraints(IValidatable validator)
    {
        -- redacted --(validator);
        -- redacted --(validator);
        -- redacted --(validator);
        -- redacted --(validator);
    }

    private static void -- redacted --(IValidatable validator)
    {
    }

    private static void -- redacted --(IValidatable validator)
    {
        -- redacted --
    }

    private static void -- redacted --(IValidatable validator)
    {
    }

    private static void -- redacted --(IValidatable validator)
    {
        validator.Property<-- redacted -->(-- redacted --)
            .Description("-- redacted --")
            .Purpose(PurposeData)
            .NotNullOrEmptyString();

        validator.Property<-- redacted -->(-- redacted --)
            .Description("-- redacted --")
            .Purpose(PurposeData + ", " + PurposeStatistics)
            .NotNull();
    }
}