using CommandLine;

namespace OptionsSeeder;

/// <summary>
/// Argument options. 
/// </summary>
public class Options : IOptions
{
    /// <summary>
    /// Boolean value indicating whether verbose logging is enabled.
    /// </summary>
    [Option(shortName: 'v', longName: "verbose", Required = false, HelpText = "Enable verbose logging")]
    public bool Verbose { get; set; } = false;

    /// <summary>
    /// Boolean value indicating whether to prompt before inserting.
    /// </summary>
    [Option(shortName: 'p', longName: "prompt", Required = false, HelpText = "Prompt before inserting")]
    public bool PromptBeforeInsert { get; set; } = false;
}