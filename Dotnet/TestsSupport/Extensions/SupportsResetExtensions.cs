using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSupport.TestClasses;

namespace TestsSupport.Extensions;

/// <summary>
/// Provides extension methods for working with objects implementing <see cref="ISupportsReset"/>.
/// </summary>
public static class SupportsResetExtensions
{
    /// <summary>
    /// Adds a <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> to a
    /// <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> root object, representing a
    /// parent-child relationship between two <see cref="ISupportsReset"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="ISupportsReset"/> object.</typeparam>
    /// <param name="supportsReset">The <see cref="ISupportsReset"/> object.</param>
    /// <param name="idRetriever">A function to retrieve the ID of the <see cref="ISupportsReset"/> object.</param>
    /// <param name="root">The root <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> object.</param>
    /// <returns>
    /// The newly created <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> representing
    /// the parent-child relationship.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="supportsReset"/> or <paramref name="idRetriever"/> is null.
    /// </exception>
    public static ParentChildRelationship<AssertableResetPropertyBehavior> Add<T>(this T supportsReset,
        Func<T, Guid> idRetriever, ParentChildRelationship<AssertableResetPropertyBehavior> root = null)
        where T : ISupportsReset
    {
        var parentChildRelationship = new ParentChildRelationship<AssertableResetPropertyBehavior>
        {
            Id = idRetriever(supportsReset)
        };

        parentChildRelationship.List.AddRange(supportsReset.GetListOfAssertableResetPropertyBehavior());
        _ = (root?.AddChild(supportsReset.GetType(), parentChildRelationship));
        return parentChildRelationship;
    }

    /// <summary>
    /// Gets a list of <see cref="AssertableResetPropertyBehavior"/> objects from an <see cref="ISupportsReset"/> object.
    /// </summary>
    /// <param name="supportsReset">The <see cref="ISupportsReset"/> object.</param>
    /// <returns>
    /// A list of <see cref="AssertableResetPropertyBehavior"/> objects representing the properties
    /// of the <see cref="ISupportsReset"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="supportsReset"/> is null.</exception>
    private static IEnumerable<AssertableResetPropertyBehavior> GetListOfAssertableResetPropertyBehavior(
        this ISupportsReset supportsReset)
    {
        return supportsReset.GetType()
            .GetPropertiesWithResetPropertyAttribute()
            .Select(tuple => new AssertableResetPropertyBehavior(tuple.Item1, supportsReset))
            .ToList();
    }

    /// <summary>
    /// Verifies the <see cref="AssertableResetPropertyBehavior"/> objects of an <see cref="ISupportsReset"/>
    /// object within a <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> root object.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="ISupportsReset"/> object.</typeparam>
    /// <param name="root">The root <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> object.</param>
    /// <param name="supportsReset">The <see cref="ISupportsReset"/> object to verify.</param>
    /// <param name="idRetriever">A function to retrieve the ID of the <see cref="ISupportsReset"/> object.</param>
    /// <param name="syncBeforeVerify">Sync the instance before calling verify.</param>
    /// <returns>
    /// The <see cref="ParentChildRelationship{AssertableResetPropertyBehavior}"/> object representing
    /// the parent-child relationship.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="root"/>, <paramref name="supportsReset"/>, or <paramref name="idRetriever"/> is null.
    /// </exception>
    /// <exception cref="AssertFailedException">
    /// Thrown when the parent-child relationship or any of the <see cref="AssertableResetPropertyBehavior"/> objects are null.
    /// </exception>
    public static ParentChildRelationship<AssertableResetPropertyBehavior> Verify<T>(
        this ParentChildRelationship<AssertableResetPropertyBehavior> root, T supportsReset, Func<T, Guid> idRetriever,
        bool syncBeforeVerify = false) where T : ISupportsReset
    {
        Assert.IsNotNull(root);
        Assert.IsNotNull(supportsReset);
        Assert.IsNotNull(idRetriever);
        var parentChildRelationship = root.Children[supportsReset.GetType()]
            .FirstOrDefault(x => x.Id == idRetriever(supportsReset));
        Assert.IsNotNull(parentChildRelationship);
        parentChildRelationship.List.ForEach(item =>
        {
            if (syncBeforeVerify)
            {
                item.Sync(supportsReset);
            }

            item.Verify();
        });
        return parentChildRelationship;
    }

    /// <summary>
    /// Determines whether a property can be written from outside its declaring type.
    /// </summary>
    /// <param name="propertyInfo">The property to check.</param>
    /// <returns><c>true</c> if the property can be written from outside; otherwise, <c>false</c>.</returns>
    public static bool CanWriteFromOutside(this PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanWrite)
        {
            return false;
        }

        var setMethod = propertyInfo.SetMethod!;
        if (setMethod.IsPrivate)
        {
            return false;
        }

        var setMethodReturnParameterModifiers = setMethod.ReturnParameter!.GetRequiredCustomModifiers();
        return !setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
    }
}