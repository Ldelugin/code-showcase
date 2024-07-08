using System;
using -- redacted --;

namespace -- redacted --;

public class ValidationErrorWithEventArgs(EventArgs eventArgs, string message) : ValidationError(message), IValidationErrorWithEventArgs
{
    public EventArgs EventArgs { get; protected set; } = eventArgs ?? throw new ArgumentNullException(nameof(eventArgs));

    public ValidationErrorWithEventArgs(EventArgs eventArgs)
        : this(eventArgs, ValidationUtility.GetMessageFromEventArgs(eventArgs))
    {
    }
}
