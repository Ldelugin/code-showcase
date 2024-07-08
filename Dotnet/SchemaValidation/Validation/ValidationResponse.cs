using System.Collections.Generic;

namespace -- redacted --;

public class ValidationResponse : IValidationResponse
{
    public bool IsValid { get; private set; }

    public IList<IValidationError> ValidationErrors { get; private set; }

    public ValidationResponse(bool isValid, IList<IValidationError> validationErrors)
    {
        this.IsValid = isValid;

        validationErrors ??= [];

        this.ValidationErrors = validationErrors;
    }
}
