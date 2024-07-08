using System;
using -- redacted --;

namespace -- redacted --;

public class ValidationErrorWithException(Exception exception, string message) : ValidationError(message), IValidationErrorWithExeption
{
    public Exception Exception { get; private set; } = exception ?? throw new ArgumentNullException(nameof(exception));

    public ValidationErrorWithException(Exception exception)
        : this(exception, ValidationUtility.GetMessageFromException(exception))
    {
    }
}
