using System;

namespace -- redacted --;

public class ValidationError(string message) : IValidationError
{
    public string Message { get; protected set; } = message ?? throw new ArgumentNullException(nameof(message));
}
