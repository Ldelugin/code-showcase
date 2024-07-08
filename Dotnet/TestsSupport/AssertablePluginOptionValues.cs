using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using -- redacted --;
using TestsSupport.Extensions;

namespace TestsSupport;

public class AssertablePluginOptionValues<T>(Expression<Func<T, object>> selector) : AssertableOptionValuesBase(
    GetFullQualifiedKeyFromPropertyName(selector))
{
    private static string GetFullQualifiedKeyFromPropertyName<TOptions, TProp>(
        Expression<Func<TOptions, TProp>> selector)
    {
        var memberInfo = selector.GetMemberInfoFromSelector();
        var builder = new StringBuilder();
        _ = builder.Append(selector.Parameters.First().Type.Name);
        _ = builder.Append(value: ':');
        _ = builder.Append(memberInfo.Name);
        var lastPartOfKey = builder.ToString();
        return $"{-- redacted --}:{Guid.NewGuid()}:{lastPartOfKey}";
    }
}