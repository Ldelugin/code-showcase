using System;
using System.Linq.Expressions;
using TestsSupport.Extensions;

namespace TestsSupport;

public class AssertableCompoundOptionValues<T>(Expression<Func<T, object>> firstSelector, Expression<Func<T, object>> secondSelector)
    : AssertableCompoundOptionValuesBase(GetFullQualifiedKeyFromPropertyName(firstSelector), GetFullQualifiedKeyFromPropertyName(secondSelector))
{
    private static string GetFullQualifiedKeyFromPropertyName<TOptions, TProp>(
        Expression<Func<TOptions, TProp>> selector)
    {
        var memberInfo = selector.GetMemberInfoFromSelector();
        return memberInfo.GetFullQualified-- redacted --OptionsKey();
    }
}