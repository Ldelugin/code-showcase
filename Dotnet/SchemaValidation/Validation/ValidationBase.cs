using System;
using System.Collections.Generic;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// Base class providing validation of <see cref="TInput"/> against a <see cref="TSchema"/>.
/// </summary>
/// <typeparam name="TSchema">The type of the schema to validate against.</typeparam>
/// <typeparam name="TInput">The type of the input to validate against the schema.</typeparam>
/// <remarks>
/// Creates a new instance of <see cref="ValidationBase{TSchema,TInput}"/>.
/// </remarks>
/// <param name="schema">Instance of <see cref="TSchema"/> to validate any input against.</param>
public abstract class ValidationBase<TSchema, TInput>(TSchema schema) : IValidate<TInput>
{
    protected readonly TSchema Schema = schema;

    /// <summary>
    /// Validate the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The <see cref="TInput"/> to validate.</param>
    /// <param name="validationErrors">
    /// List of <see cref="IValidationError"/> that could have been encountered while
    /// validating the <paramref name="input"/>.
    /// </param>
    /// <returns>Returns true if the given <paramref name="input"/> is valid; otherwise false.</returns>
    public bool Validate(TInput input, out IList<IValidationError> validationErrors)
    {
        validationErrors = [];

        var hasValidInput = ValidateInput(input, validationErrors);
        var hasValidSchema = ValidateSchema(this.Schema, validationErrors);

        if (!hasValidInput || !hasValidSchema)
        {
            return false;
        }

        var isValid = false;
        try
        {
            isValid = this.ValidateInput(input, out var errors);
            validationErrors = errors;
        }
        catch (Exception e)
        {
            ValidationUtility.AddValidationErrorToList(validationErrors, e);
        }

        return isValid;
    }

    /// <summary>
    /// Validate the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The <see cref="TInput"/> to validate.</param>
    /// <returns>
    /// Returns a <see cref="IValidationResponse"/> containing information about the validation process.
    /// </returns>
    public IValidationResponse Validate(TInput input)
    {
        var isValid = this.Validate(input, out var validationErrors);
        return new ValidationResponse(isValid, validationErrors);
    }

    /// <summary>
    /// Convert the given <paramref name="eventArgs"/> into a <see cref="IValidationError"/>.
    /// </summary>
    /// <param name="eventArgs">The given <see cref="EventArgs"/> to convert.</param>
    /// <param name="message">
    /// Optional custom message to provide. If not provided then the normal message originally provided
    /// to the event args is used.
    /// </param>
    /// <returns>Returns the converted <see cref="IValidationError"/>.</returns>
    protected IValidationError ConvertEventArgsToValidationError(EventArgs eventArgs, string message = null)
    {
        return message == null
            ? new ValidationErrorWithEventArgs(eventArgs)
            : new ValidationErrorWithEventArgs(eventArgs, message);
    }

    /// <summary>
    /// Validate the given <paramref name="input"/> against the provided <see cref="Schema"/>.
    /// </summary>
    /// <param name="input">The <see cref="TInput"/> to validate.</param>
    /// <param name="validationErrors">
    /// List of <see cref="IValidationError"/> that could have been encountered while
    /// validating the <paramref name="input"/>.
    /// </param>
    /// <returns>
    /// Returns true if the given <paramref name="input"/> is valid against the
    /// provided <see cref="Schema"/>; otherwise false.
    /// </returns>
    protected abstract bool ValidateInput(TInput input, out IList<IValidationError> validationErrors);

    /// <summary>
    /// Validate the given <paramref name="input"/> to check if it's valid to be used.
    /// </summary>
    /// <param name="input">The <see cref="TInput"/> to validate.</param>
    /// <param name="validationErrors">
    /// The list of <see cref="IValidationError"/> to add the exception to if the
    /// given <paramref name="input"/> is not valid.
    /// </param>
    /// <returns>Returns true if the given <paramref name="input"/> is valid to be used; otherwise false.</returns>
    private static bool ValidateInput(TInput input, IList<IValidationError> validationErrors)
    {
        if (input == null)
        {
            ValidationUtility.AddValidationErrorToList(validationErrors, new ArgumentNullException(nameof(input)));
            return false;
        }

        if (input is string inputString && string.IsNullOrWhiteSpace(inputString))
        {
            ValidationUtility.AddValidationErrorToList(validationErrors,
                new ArgumentException("The input as a string cannot be null, empty or whitespace"));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate the given <paramref name="schema"/> to check if it's valid to be used.
    /// </summary>
    /// <param name="schema">The <see cref="TSchema"/> to validate.</param>
    /// <param name="validationErrors">
    /// The list of <see cref="IValidationError"/> to add the exception to if the
    /// given <paramref name="schema"/> is not valid.
    /// </param>
    /// <returns>Returns true if the given <paramref name="schema"/> is valid to be used; otherwise false.</returns>
    private static bool ValidateSchema(TSchema schema, IList<IValidationError> validationErrors)
    {
        if (schema != null)
        {
            return true;
        }

        ValidationUtility.AddValidationErrorToList(validationErrors, new ArgumentNullException(nameof(schema)));
        return false;
    }
}
