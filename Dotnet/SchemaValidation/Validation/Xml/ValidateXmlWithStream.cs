using System;
using -- redacted --;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;

namespace -- redacted --;

/// <summary>
/// Class providing validation of <see cref="Stream"/> against a <see cref="XmlSchemaSet"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ValidateXmlWithStream"/>.
/// </remarks>
/// <param name="schema">Instance of <see cref="XmlSchemaSet"/> to validate any input against.</param>
public class ValidateXmlWithStream(XmlSchemaSet schema) : ValidationBase<XmlSchemaSet, Stream>(schema)
{
    private const string XPath = "XPath";

    /// <summary>
    /// Validate the given <paramref name="input"/> against the
    /// provided <see cref="ValidationBase{TSchema,TInput}.Schema"/>.
    /// </summary>
    /// <param name="input">The <see cref="Stream"/> to validate.</param>
    /// <param name="validationErrors">
    /// List of <see cref="IValidationError"/> that could have been encountered while
    /// validating the <paramref name="input"/>.
    /// </param>
    /// <returns>
    /// Returns true if the given <paramref name="input"/> is valid against the
    /// provided <see cref="ValidationBase{TSchema,TInput}.Schema"/>; otherwise false.
    /// </returns>
    protected override bool ValidateInput(Stream input, out IList<IValidationError> validationErrors)
    {
        var errors = new List<IValidationError>();

        void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            var path = sender switch
            {
                XElement element => element.GetAbsoluteXPath(),
                XAttribute attribute => attribute.getAbsoluteXPath(),
                _ => throw new ArgumentException($"Unknown type of sender: {sender.GetType()}")
            };

            e.Exception.Data.Add(XPath, path);
            errors.Add(this.ConvertEventArgsToValidationError(e,
                $"{e.Exception.Data[XPath]}: {e.Message}"));
        }


        foreach (XmlSchema s in this.Schema.Schemas())
        {
            WriteStreamIntoSchema(input, s);
        }

        if (input.Position > 0)
        {
            _ = input.Seek(offset: 0, origin: SeekOrigin.Begin);
        }

        var document = XDocument.Load(input);
        document.Validate(this.Schema, ValidationEventHandler);

        validationErrors = errors;
        return validationErrors.Count == 0;
    }

    /// <summary>
    /// Write the given <paramref name="stream"/> into the given <see cref="schema"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to write into the <paramref name="schema"/>.</param>
    /// <param name="schema">The <see cref="schema"/> to write the <paramref name="stream"/> into.</param>
    private static void WriteStreamIntoSchema(Stream stream, XmlSchema schema)
    {
        if (stream.Position != 0)
        {
            _ = stream.Seek(offset: 0, origin: SeekOrigin.Begin);
        }

        var streamReader = new StreamReader(stream);
        _ = streamReader.ReadToEnd();

        var ms = new MemoryStream();
        schema.Write(ms);
        _ = ms.Seek(offset: 0, loc: SeekOrigin.Begin);
        streamReader = new StreamReader(ms);
        _ = streamReader.ReadToEnd();

        streamReader.Close();
        ms.Close();

        _ = stream.Seek(offset: 0, origin: SeekOrigin.Begin);
    }
}
