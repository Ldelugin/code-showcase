using System;

namespace -- redacted --;

public class ValidationEventArgs(string message) : EventArgs
{
    public string Message { get; private set; } = message ?? throw new ArgumentNullException(nameof(message));
}
