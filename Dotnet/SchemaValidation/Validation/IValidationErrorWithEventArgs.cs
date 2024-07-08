using System;

namespace -- redacted --;

/// <summary>
/// The Validation Error With EventArgs interface. Inherts the <see cref="IValidationError"/> and adds an <see cref="System.EventArgs"/> property.
/// </summary>
public interface IValidationErrorWithEventArgs : IValidationError
{
    /// <summary>
    /// The EventArgs that can be throwned when validation input.
    /// </summary>
    /// /// <returns>The <see cref="System.EventArgs"/> that is raised when validating the input.</returns>
    EventArgs EventArgs { get; }
}
