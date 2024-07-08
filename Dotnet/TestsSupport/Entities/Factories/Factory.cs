using System;
using TestsSupport.Entities.Extensions;

namespace TestsSupport.Entities.Factories;

/// <summary>
/// Factory class to create entities with.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to create.</typeparam>
public class Factory<TEntity> where TEntity : new()
{
    /// <summary>
    /// Creates an instance of <see cref="TEntity"/>.
    /// </summary>
    /// <example>
    /// Example with multiple chained With methods.
    /// <code>
    /// -- redacted --
    /// </code>
    /// Example with one With methods, using multiple actions.
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="configureDelegates">
    /// An array of actions that operates on the given <see cref="TEntity"/>.
    /// </param>
    /// <returns>Returns a new instance of <see cref="TEntity"/>.</returns>
    public TEntity Create(params Action<TEntity>[] configureDelegates)
        => new TEntity().With(configureDelegates);
}
