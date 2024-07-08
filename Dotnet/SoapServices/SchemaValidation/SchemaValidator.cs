using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using -- redacted --;
using -- redacted --;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Validates a incoming message against a <see cref="XmlSchemaSet"/>.
/// </summary>
/// <remarks>
/// Creates instance of <see cref="SchemaValidator"/>.
/// </remarks>
/// <param name="validator">
/// Provide an instance of <see cref="IValidate{TInput}"/>.
/// </param>
public class SchemaValidator(IValidate<Stream> validator)
{
    /// <summary>
    /// Instance of IValidate that handles the validation with a <see cref="Stream"/> 
    /// against a <see cref="XmlSchemaSet"/>.
    /// </summary>
    private readonly IValidate<Stream> validator = validator;

    /// <summary>
    /// Validates a provided <paramref name="message"/>.
    /// </summary>
    /// <param name="message">
    /// The message that's need's to be validated.
    /// </param>
    public void ValidateMessage(ref Message message) => message = this.CreateValidMessage(message);

    /// <summary>
    /// Converts the provided message into a <see cref="XDocument"/>, which 
    /// get's validated. After validation and no issues where found the 
    /// <see cref="XDocument"/> get's converted back into a valid message.        
    /// </summary>
    /// <param name="message">
    /// The message to convert and validate.
    /// </param>
    /// <returns>
    /// The validated message.
    /// </returns>
    private Message CreateValidMessage(Message message)
    {
        var document = ConvertMessageToXDocument(message);
        this.PerformValidation(document);
        return ConvertXDocumentToMessage(document, message);
    }

    /// <summary>
    /// Convert the provided <see cref="Message"/> into a <see cref="XDocument"/>.
    /// </summary>
    /// <param name="message">
    /// The message to convert.
    /// </param>
    /// <returns>
    /// The <see cref="XDocument"/> converted from the <paramref name="message"/>.
    /// </returns>
    private static XDocument ConvertMessageToXDocument(Message message)
    {
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream);
        message.WriteMessage(writer);
        writer.Flush();

        var body = Encoding.UTF8.GetString(stream.ToArray()).RemoveUTF8Preamble();

        using var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return XDocument.Load(bodyStream);
    }

    /// <summary>
    /// Convert the provided <paramref name="document"/> into a <see cref="Message"/>.
    /// </summary>
    /// <param name="document">
    /// The <see cref="XDocument"/> to convert.
    /// </param>
    /// <param name="oldMessage">
    /// The old message, which contains vital information for converting the <paramref name="document"/>
    /// back into a valid message.
    /// </param>
    /// <returns>
    /// The converted <paramref name="document"/> into a valid message.
    /// </returns>
    private static Message ConvertXDocumentToMessage(XDocument document, Message oldMessage)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (oldMessage == null)
        {
            throw new ArgumentNullException(nameof(oldMessage));
        }

        using var stream = new MemoryStream();
        document.Save(stream);
        _ = stream.Seek(0, SeekOrigin.Begin);

        var quotas = new XmlDictionaryReaderQuotas() { MaxStringContentLength = int.MaxValue };
        var reader = XmlDictionaryReader.CreateTextReader(stream.ToArray(), quotas);
        var newMessage = Message.CreateMessage(reader, int.MaxValue, oldMessage.Version);
        newMessage.Properties.CopyProperties(oldMessage.Properties);
        return newMessage;
    }

    /// <summary>
    /// Performs the validation on the <paramref name="document"/>.
    /// If the validationResponse conclude that it's not valid it will throw
    /// a <see cref="BadRequestFaultException"/>.
    /// </summary>
    /// <param name="document">
    /// The converted <see cref="XDocument"/> of the incoming message that's need
    /// to be validated.
    /// </param>
    /// <exception cref="BadRequestFaultException">
    /// Will be throwed when the validation of the <paramref name="document"/> 
    /// doesn't succeed.
    /// </exception>
    private void PerformValidation(XDocument document)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        using var stream = new MemoryStream();
        document.Save(stream);

        if (stream.Position > 0)
        {
            _ = stream.Seek(0, SeekOrigin.Begin);
        }

        var validationResponse = this.validator.Validate(stream);

        if (!validationResponse.IsValid)
        {
            var invalidDetail = new InvalidDetail();
            var reasons = new List<string>();
            foreach (var validationError in validationResponse.ValidationErrors)
            {
                reasons.Add(validationError.Message);
            }
            invalidDetail.Reasons = [.. reasons];
            throw new BadRequestFaultException(invalidDetail);
        }
    }
}
