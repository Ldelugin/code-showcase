using System;

namespace TestsSupport.Entities.Factories;

/// <summary>
/// Factory class to create entities while passing arguments to its constructor.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to create.</typeparam>
public class FactoryWithArguments<TEntity>
{
    /// <summary>
    /// Creates a new instance of <see cref="TEntity"/> with the option to pass arguments for the constructor.
    /// </summary>
    /// <example>
    /// Example how to use and why the method exists.
    /// <code>
    /// // The only option to set the Id property is to pass it in the constructor.
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="arguments">An array of constructor arguments.</param>
    /// <returns>Returns a new instance of <see cref="TEntity"/>.</returns>
    public TEntity CreateWithArguments(params object[] arguments)
        => (TEntity)Activator.CreateInstance(typeof(TEntity), arguments);
}
