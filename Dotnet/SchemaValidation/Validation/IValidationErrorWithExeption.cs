using System;

namespace -- redacted --;

/// <summary>
/// The Validation Error With Exception interface. Inherts the <see cref="IValidationError"/> and adds an <see cref="System.Exception"/> property.
/// </summary>
public interface IValidationErrorWithExeption : IValidationError
{
    /// <summary>
    /// The Exception that can be throwned when validation input.
    /// </summary>
    /// <returns>The <see cref="System.Exception"/> that is thrown when validating the input.</returns>
    Exception Exception { get; }
}
