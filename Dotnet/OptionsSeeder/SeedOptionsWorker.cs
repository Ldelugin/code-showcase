using System.Reflection;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OptionsSeeder;

/// <summary>
/// Worker that seeds the options in the database.
/// </summary>
public class SeedOptionsWorker : BackgroundService
{
    /// <summary>
    /// Section name in the configuration file that contains the seeded options.
    /// </summary>
    private const string SeededOptionsSectionName = "SeededOptions";
    private const string OptionsPart = "options";

    private readonly ILogger<SeedOptionsWorker> logger;
    private readonly IConfiguration configuration;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IEfSystemConfigurationProvider systemConfigurationProvider;
    private readonly IOptions options;
    private readonly Type optionsType = typeof(-- redacted --);
    private readonly Dictionary<string, PropertyInfo> optionsPropertiesDictionary = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedOptionsWorker"/> class.
    /// </summary>
    /// <param name="configuration">
    /// Instance of <see cref="IConfiguration"/> which also implements <see cref="IConfigurationRoot"/> and
    /// provides an instance of <see cref="IEfSystemConfigurationProvider"/>.
    /// </param>
    /// <param name="logger">Instance of <see cref="ILogger{TCategoryName}"/>.</param>
    /// <param name="applicationLifetime">
    /// Instance of <see cref="IHostApplicationLifetime"/> to be able to stop the application.
    /// </param>
    /// <param name="options">
    /// Instance of <see cref="IOptions"/> to be able to read the options from the arguments.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configuration"/>, <paramref name="logger"/>, <paramref name="options"/>
    /// <paramref name="applicationLifetime"/> or <see cref="systemConfigurationProvider"/> is null.
    /// </exception>
    public SeedOptionsWorker(IConfiguration configuration, ILogger<SeedOptionsWorker> logger,
        IHostApplicationLifetime applicationLifetime, IOptions options)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        this.systemConfigurationProvider = configuration.GetIEfSystemConfigurationProvider() ??
                                           throw new ArgumentNullException(nameof(configuration));
        this.options = options ?? throw new ArgumentNullException(nameof(options));

        foreach (var propertyInfo in this.optionsType.GetProperties())
        {
            var propertyName = propertyInfo.Name;
            var field = propertyInfo.PropertyType.GetField(nameof(-- redacted --),
                BindingFlags.Static | BindingFlags.Public);

            if (field != null)
            {
                var fieldValue = field.GetValue(obj: null);
                if (fieldValue != null)
                {
                    propertyName = fieldValue.ToString();
                }
            }

            if (propertyName != null)
            {
                this.optionsPropertiesDictionary.Add(propertyName, propertyInfo);
            }
        }
    }

    /// <summary>
    /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts.
    /// The implementation should return a task that represents
    /// the lifetime of the long running operation(s) being performed.
    /// </summary>
    /// <param name="stoppingToken">
    /// Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Start seeding options");
        await Task.Yield();

        try
        {
            var configurationSections = GetAllNestedChildren(
                    this.configuration.GetSection(SeededOptionsSectionName))
                .ToArray();

            if (this.options.Verbose)
            {
                this.logger.LogInformation("The following options are seeded:");
                configurationSections.ToList().ForEach(child => this.logger.LogInformation(
                    "  {Path} = {Value}", child.Path, child.Value));
            }

            var validationResult = this.ValidateAllOptions(configurationSections);
            var areAllOptionsValid = validationResult.All(kvp => kvp.Value.IsValid);
            this.logger.LogInformation("End validating options, all options are valid: {Valid}",
                areAllOptionsValid ? "Yes" : "No");

            if (!areAllOptionsValid)
            {
                foreach (var (key, value) in validationResult.Where(kvp => !kvp.Value.IsValid))
                {
                    this.logger.LogError("The option '{Key}' is not valid. Reason: {InvalidReason}",
                        key, value.InvalidReason);
                }

                return;
            }

            this.InsertOptions(configurationSections);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Error while seeding options");
        }
        finally
        {
            this.applicationLifetime.StopApplication();
        }
    }

    /// <summary>
    /// Get all nested children of the configuration section.
    /// </summary>
    /// <param name="section">
    /// The configuration section to get all nested children from.
    /// </param>
    /// <returns>
    /// IEnumerable of <see cref="IConfigurationSection"/> that contains all nested children.
    /// </returns>
    private static IEnumerable<IConfigurationSection> GetAllNestedChildren(IConfigurationSection section)
    {
        foreach (var child in section.GetChildren())
        {
            if (child.Value != null)
            {
                yield return child;
            }

            foreach (var grandChild in GetAllNestedChildren(child))
            {
                if (grandChild.Value != null)
                {
                    yield return grandChild;
                }
            }
        }
    }

    /// <summary>
    /// Insert the options in the database.
    /// </summary>
    /// <param name="configurationSections">
    /// IEnumerable of <see cref="IConfigurationSection"/> that contains the options.
    /// </param>
    private void InsertOptions(IEnumerable<IConfigurationSection> configurationSections)
    {
        var optionsToInsert = configurationSections.ToDictionary(
            section => section.Path.Replace(SeededOptionsSectionName, -- redacted --),
            section => section.Value);

        if (optionsToInsert.Count == 0)
        {
            this.logger.LogWarning("No options to insert");
            return;
        }

        if (this.options.Verbose || this.options.PromptBeforeInsert)
        {
            this.logger.LogInformation("About to insert the following options (values for existing keys will not be updated):");
            optionsToInsert.ToList().ForEach(kvp => this.logger.LogInformation(
                "  {Key} = {Value}", kvp.Key, kvp.Value));
        }

        if (!this.options.PromptBeforeInsert || this.PromptBeforeInsert())
        {
            this.systemConfigurationProvider.ReadAndWriteConfigurations(optionsToInsert);
        }
    }

    /// <summary>
    /// Prompt the user whether the options should be inserted.
    /// </summary>
    /// <returns>Returns true if the user wants to insert the options, otherwise false.</returns>
    private bool PromptBeforeInsert()
    {
        ConsoleKeyInfo? answer = null;

        while (answer is not { Key: ConsoleKey.Enter or ConsoleKey.Y or ConsoleKey.N })
        {
            this.logger.LogInformation("Do you want to insert the options? (Y/N)");
            answer = Console.ReadKey();
        }

        return answer.Value.Key == ConsoleKey.Y;
    }

    /// <summary>
    /// Validate whether all options are known options and whether the values are valid.
    /// </summary>
    /// <param name="configurationSections">The configuration sections that contain the options.</param>
    /// <returns>
    /// A dictionary with the key as the option name and the value as a tuple with a bool that indicates
    /// whether the option is valid.
    /// </returns>
    private Dictionary<string, (bool IsValid, string? InvalidReason)> ValidateAllOptions(
        IEnumerable<IConfigurationSection> configurationSections)
    {
        this.logger.LogInformation("Start validating options");
        var validationResult = new Dictionary<string, (bool IsValid, string? InvalidReason)>();

        foreach (var configurationSection in configurationSections)
        {
            var isValid = this.ValidateOption(configurationSection, out var invalidReason);
            validationResult.Add(configurationSection.Path, (isValid, invalidReason));

            if (this.options.Verbose)
            {
                this.logger.LogInformation("The option '{Key}' is valid: {Valid}",
                    configurationSection.Path, isValid ? "Yes" : "No");
            }
        }

        return validationResult;
    }

    /// <summary>
    /// Validate whether the option is a known option and whether the value is valid.
    /// </summary>
    /// <param name="configurationSection">The configuration section that contains the option.</param>
    /// <param name="invalidReason">The reason why the option is invalid. This is null if the option is valid.</param>
    /// <returns>Returns true if the option is valid, otherwise false.</returns>
    private bool ValidateOption(IConfigurationSection configurationSection, out string? invalidReason)
    {
        // Take the second part of the path, this is the options group name
        var optionsGroupName = configurationSection.Path.Split(ConfigurationPath.KeyDelimiter)[1];
        var optionsName = configurationSection.Key;
        var optionsValue = configurationSection.Value;

        var groupOptionPropertyInfo = this.optionsPropertiesDictionary
            .FirstOrDefault(kvp => CompareOptionsGroupName(kvp.Key, optionsGroupName));

        if (groupOptionPropertyInfo.Value == null)
        {
            invalidReason = $"The options group name '{optionsGroupName}' is not valid.";
            return false;
        }

        var optionPropertyInfo = Array.Find(groupOptionPropertyInfo.Value?.PropertyType.GetProperties() ?? [],
            p => p.Name.Equals(optionsName, StringComparison.OrdinalIgnoreCase));

        if (optionPropertyInfo == null)
        {
            invalidReason = $"The options name '{optionsName}' of '{optionsGroupName}' is not valid.";
            return false;
        }

        var isValueValid = !string.IsNullOrWhiteSpace(optionsValue);
        invalidReason = isValueValid
            ? null
            : $"The value '{optionsValue}' of '{optionsGroupName}:{optionsName}' is not valid.";

        return isValueValid;
    }

    /// <summary>
    /// Compare the property info name with the options group name of the options.
    /// </summary>
    /// <param name="propertyInfoName">The property info name.</param>
    /// <param name="optionsGroupName">The options group name.</param>
    /// <returns>Returns true if the property info name is equal to the options group name; otherwise, false.</returns>
    private static bool CompareOptionsGroupName(string propertyInfoName, string optionsGroupName)
    {
        return Normalize(propertyInfoName) == Normalize(optionsGroupName);

        static string Normalize(string value)
        {
            var normalized = value.ToLower();
            if (normalized.EndsWith(OptionsPart))
            {
                normalized = normalized[..^OptionsPart.Length];
            }
            return normalized;
        }
    }
}