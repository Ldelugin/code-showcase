using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace -- redacted --;

public static class ValidationUtility
{
    public static IValidationResponse GetValidationResponse(bool isValid, string[] validationErrors)
    {
        var errors = validationErrors?.Select(e => new ValidationError(e)).Cast<IValidationError>().ToList() ?? [];
        return new ValidationResponse(isValid, errors);
    }

    public static string FormatValidationErrorsIntoOneString(IList<IValidationError> validationErrors)
    {
        var errors = FormatValidationErrors(validationErrors);
        var stringBuilder = new StringBuilder();
        foreach (var error in errors)
        {
            _ = stringBuilder.AppendLine(error);
        }
        return stringBuilder.ToString();
    }


    public static string[] FormatValidationErrors(IList<IValidationError> validationErrors)
    {
        if (validationErrors == null || !validationErrors.Any())
        {
            return [];
        }

        var errors = new string[validationErrors.Count];

        for (var i = 0; i < validationErrors.Count; i++)
        {
            var error = validationErrors[i];
            if (error == null)
            {
                continue;
            }

            var errorMessage = string.Empty;

            var errorWithEventArgs = error as IValidationErrorWithEventArgs;
            var errorWithExeption = error as IValidationErrorWithExeption;

            if (errorWithEventArgs != null && errorWithExeption == null)
            {
                errorMessage = FormatValidationError(errorWithEventArgs, errorWithEventArgs.Message, errorWithEventArgs.EventArgs);
            }

            if (errorWithExeption != null && errorWithEventArgs == null)
            {
                errorMessage = FormatValidationError(errorWithExeption, errorWithExeption.Message, errorWithExeption.Exception);
            }

            if (errorWithEventArgs == null && errorWithExeption == null)
            {
                errorMessage = FormatValidationError(error, error.Message);
            }

            errors[i] = errorMessage;
        }

        return errors;
    }

    public static string FormatValidationError(IValidationError validationError, string message, object optional = null)
    {
        if (validationError == null)
        {
            return null;
        }

        var builder = new StringBuilder();
        _ = builder.Append(value: '[');
        _ = builder.Append(validationError.GetType());
        _ = builder.Append("] ");
        _ = builder.Append("Message: ");
        _ = builder.Append(message);

        if (optional != null)
        {
            _ = builder.Append(" || ");
            _ = builder.Append(optional.GetType());
            _ = builder.Append(": ");
            _ = builder.Append(optional);
        }

        return builder.ToString();
    }

    public static string GetMessageFromEventArgs(EventArgs eventArgs)
    {
        var message = string.Empty;

        if (eventArgs != null)
        {
            if (eventArgs is ValidationEventArgs validationEventArgs)
            {
                message = validationEventArgs.Message;
            }

            if (eventArgs is System.Xml.Schema.ValidationEventArgs xmlValidationEventArgs)
            {
                message = xmlValidationEventArgs.Message;
            }

            if (eventArgs is Newtonsoft.Json.Schema.SchemaValidationEventArgs jsonValidationEventArgs)
            {
                message = jsonValidationEventArgs.Message;
            }
        }

        return message;
    }

    public static string GetMessageFromException(Exception exception) => exception != null ? exception.Message : string.Empty;

    public static void AddValidationErrorToList(IList<IValidationError> list, EventArgs eventArgs, string message)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        list.Add(GetValidationError(eventArgs, message));
    }

    public static void AddValidationErrorToList(IList<IValidationError> list, Exception exception)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        list.Add(GetValidationError(exception));
    }

    public static void AddValidationErrorToList(IList<IValidationError> list, Exception exception, string message)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        list.Add(GetValidationError(exception, message));
    }

    public static IValidationError GetValidationError(EventArgs eventArgs, string message)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return new ValidationErrorWithEventArgs(eventArgs, message);
    }

    public static IValidationError GetValidationError(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        var message = exception.Message;
        return new ValidationErrorWithException(new ValidationException(message, exception));
    }

    public static IValidationError GetValidationError(Exception innerException, string message)
    {
        if (innerException == null)
        {
            throw new ArgumentNullException(nameof(innerException));
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return new ValidationErrorWithException(new ValidationException(message, innerException));
    }
}
