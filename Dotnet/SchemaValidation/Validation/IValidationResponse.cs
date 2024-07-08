using System.Collections.Generic;

namespace -- redacted --;

/// <summary>
/// The validation response interface. Used to give a response back on the validation request. 
/// </summary>
public interface IValidationResponse
{
    /// <summary>
    /// This is the answer whether the input that needed to be validated is valid.
    /// </summary>
    /// <returns>When true the input is valid, when false the input is not valid.</returns>
    bool IsValid { get; }

    /// <summary>
    /// A List of validation errors.
    /// </summary>
    /// <returns>A list of <see cref="IValidationError"/>.</returns>
    IList<IValidationError> ValidationErrors { get; }
}
