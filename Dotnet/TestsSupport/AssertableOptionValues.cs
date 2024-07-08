using System;
using System.Linq.Expressions;

namespace TestsSupport;

/// <summary>
/// Assertable values for one layer deep nesting.
/// </summary>
/// <typeparam name="T">The options type directly below -- redacted -- (for example: -- redacted --).</typeparam>
public class AssertableOptionValues<T>(Expression<Func<T, object>> selector) : AssertableOptionValuesBase(GetFullQualifiedKeyFromPropertyName(selector, typeof(T)))
{
}

/// <summary>
/// Assertable values for two layers deep nesting.
/// </summary>
/// <typeparam name="T1">The options type directly below -- redacted -- (for example: -- redacted --).</typeparam>
/// <typeparam name="T2">
/// The nested or named options nested inside the T1 option (for example: -- redacted --
/// that are nested in -- redacted --).
/// </typeparam>
public class AssertableOptionValues<T1, T2>(Expression<Func<T2, object>> selector) : AssertableOptionValuesBase(GetFullQualifiedKeyFromPropertyName(selector, typeof(T1), typeof(T2)))
{
}

/// <summary>
/// Assertable values for three layers deep nesting.
/// </summary>
/// <typeparam name="T1">The options type directly below -- redacted -- (for example: -- redacted --).</typeparam>
/// <typeparam name="T2">
/// The nested or named options nested inside the T1 options
/// (for example: -- redacted -- that are nested in -- redacted --).
/// </typeparam>
/// <typeparam name="T3">
/// The nested or named options nested inside the T2 options
/// (for example: -- redacted -- that are nested in -- redacted --).
/// </typeparam>
public class AssertableOptionValues<T1, T2, T3>(Expression<Func<T3, object>> selector) : AssertableOptionValuesBase(GetFullQualifiedKeyFromPropertyName(selector, typeof(T1), typeof(T2), typeof(T3)))
{
}
