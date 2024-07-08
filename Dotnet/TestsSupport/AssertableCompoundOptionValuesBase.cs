using System.Collections.Generic;

namespace TestsSupport;

public abstract class AssertableCompoundOptionValuesBase(string firstKey, string secondKey)
{
    public string FirstKey { get; } = firstKey;
    public string SecondKey { get; } = secondKey;
    public List<(string FirstValue, string SecondValue, bool ExpectedToFail)> Arguments { get; } = [];

    public AssertableCompoundOptionValuesBase Assert(string firstValue, string secondValue, bool expectedToFail)
    {
        this.Arguments.Add((firstValue, secondValue, expectedToFail));
        return this;
    }
}