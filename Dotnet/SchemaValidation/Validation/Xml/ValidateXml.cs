using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace -- redacted --;

/// <summary>
/// Class providing validation of <see cref="string"/> against a <see cref="XmlSchemaSet"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ValidateXml"/>.
/// </remarks>
/// <param name="schema">Instance of <see cref="XmlSchemaSet"/> to validate any input against.</param>
public class ValidateXml(XmlSchemaSet schema) : ValidationBase<XmlSchemaSet, string>(schema), IValidateXml
{

    /// <summary>
    /// Validate the given <paramref name="input"/> against the
    /// provided <see cref="ValidationBase{TSchema,TInput}.Schema"/>.
    /// </summary>
    /// <param name="input">The <see cref="string"/> to validate.</param>
    /// <param name="validationErrors">
    /// List of <see cref="IValidationError"/> that could have been encountered while
    /// validating the <paramref name="input"/>.
    /// </param>
    /// <returns>
    /// Returns true if the given <paramref name="input"/> is valid against the
    /// provided <see cref="ValidationBase{TSchema,TInput}.Schema"/>; otherwise false.
    /// </returns>
    protected override bool ValidateInput(string input, out IList<IValidationError> validationErrors)
    {
        var errors = new List<IValidationError>();
        void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            errors.Add(this.ConvertEventArgsToValidationError(e));
        }

        var document = new XmlDocument();
        document.LoadXml(input);

        var readerSettings = new XmlReaderSettings();
        readerSettings.Schemas.Add(this.Schema);
        readerSettings.Schemas.Compile();
        readerSettings.ValidationType = ValidationType.Schema;
        readerSettings.ConformanceLevel = ConformanceLevel.Auto;
        readerSettings.ValidationEventHandler += ValidationEventHandler;

        using (var reader = XmlReader.Create(new XmlNodeReader(document), readerSettings))
        {
            try
            {
                while (reader.Read())
                {
                    // No need to do something here, just read the whole given input.
                }
            }
            finally
            {
                readerSettings.ValidationEventHandler -= ValidationEventHandler;
            }
        }

        validationErrors = errors;
        return validationErrors.Count == 0;
    }
}
