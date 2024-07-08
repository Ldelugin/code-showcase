namespace -- redacted --;

/// <summary>
/// The validate interface. Used to validate an input against a schema.
/// </summary>
/// <typeparam name="TInput">The input you want to validate.</typeparam>
public interface IValidate<in TInput>
{
    /// <summary>
    /// The validate method, validates the input against the provided schema. Returns a <see cref="IValidationResponse"/> with the response of the validation process.
    /// </summary>        
    /// <param name="input">The input that you want to validate.</param>
    /// <returns>A <see cref="IValidationResponse"/>. Contains a IsValid bool and <see cref="IValidationError"/> list.</returns>
    IValidationResponse Validate(TInput input);
}
