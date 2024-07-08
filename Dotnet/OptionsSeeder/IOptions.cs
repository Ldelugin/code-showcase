namespace OptionsSeeder;

/// <summary>
/// Interface for the argument options.
/// </summary>
public interface IOptions
{
    /// <summary>
    /// Boolean value indicating whether verbose logging is enabled.
    /// </summary>
    bool Verbose { get; set; }

    /// <summary>
    /// Boolean value indicating whether to prompt before inserting.
    /// </summary>
    bool PromptBeforeInsert { get; set; }
}