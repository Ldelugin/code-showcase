using System;
using System.Collections.Generic;
using TestsSupport.Entities.Factories;

namespace TestsSupport.Entities;

/// <summary>
/// The entity class that exposes a factory to create entities with.
/// </summary>
public static partial class Entity
{
    /// <summary>
    /// Create a list containing <paramref name="total"/> number of <typeparamref name="TEntity"/> instances.
    /// </summary>
    /// <param name="createDelegate">
    /// The delegate that will provide a new instance of <typeparamref name="TEntity"/>.
    /// </param>
    /// <param name="total">
    /// The number of <typeparamref name="TEntity"/> instances that the list should contain.
    /// </param>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <returns>List containing <paramref name="total"/> number of <typeparamref name="TEntity"/> instances</returns>
    public static List<TEntity> List<TEntity>(Func<TEntity> createDelegate, int total) where TEntity : class
    {
        if (total < 0)
        {
            return [];
        }

        var list = new List<TEntity>(total);
        for (var i = 0; i < total; i++)
        {
            list.Add(createDelegate.Invoke());
        }

        return list;
    }

    /// <summary>
    /// Create a list containing <paramref name="total"/> number of <typeparamref name="TEntity"/> instances.
    /// </summary>
    /// <param name="createDelegate">
    /// The delegate that will provide a new instance of <typeparamref name="TEntity"/>.
    /// </param>
    /// <param name="total">
    /// The number of <typeparamref name="TEntity"/> instances that the list should contain.
    /// </param>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <returns>List containing <paramref name="total"/> number of <typeparamref name="TEntity"/> instances</returns>
    public static List<TEntity> List<TEntity>(Func<int, TEntity> createDelegate, int total) where TEntity : class
    {
        if (total < 0)
        {
            return [];
        }

        var list = new List<TEntity>(total);
        for (var i = 0; i < total; i++)
        {
            list.Add(createDelegate.Invoke(i));
        }

        return list;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Factory{TEntity}"/>.
    /// </summary>
    /// <example>
    /// Example how to use the Entity.Factory without extension method,
    /// use this for types that are rarely created.
    /// <code>
    /// Entity.Factory{Passage}().Create();
    /// </code>
    /// Example how to use the Entity.Factory with an extension method,
    /// which can be used for types that are created a lot.
    /// <code>
    /// // Create a new class named `Entity.Whatever` and define it like this.
    /// public static partial class Entity
    /// {
    ///     public static Factory{Passage} Passage { get; } = Factory{Passage}();
    /// }
    ///
    /// // The above partial extension class adds support to create a Passage with the following syntax:
    /// Entity.Passage.Create();
    /// </code>
    /// </example>
    /// <typeparam name="TEntity">
    /// The type of entity this <see cref="Factory{TEntity}"/> should create.
    /// </typeparam>
    /// <returns>Returns a new instance of <see cref="Factory{TEntity}"/>.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static Factory<TEntity> Factory<TEntity>() where TEntity : new()
        => new();

    /// <summary>
    /// Creates a new instance of <see cref="FactoryWithArguments{TEntity}"/>.
    /// </summary>
    /// <example>
    /// Example how to use on objects without a public parameterless constructor.
    /// <code>
    /// // An entity that does not have a public parameterless constructor.
    /// public class DbEntity
    /// {
    ///     public DbEntity(string name)
    ///     {
    ///         this.name = name;
    ///     }
    ///
    ///     public string Name { get; private set; }
    /// }
    ///
    /// Entity.FactoryWithArguments{DbEntity}().CreateWithArguments("MyName");
    /// </code>
    /// Example how to use this with an extension method.
    /// <code>
    /// // Create a new class named `Entity.Whatever` and define it like this.
    /// public static partial class Entity
    /// {
    ///     public static FactoryWithArguments{DbEntity} DbEntity { get; } = FactoryWithArguments{DbEntity}();
    /// }
    ///
    /// Entity.DbEntity.CreateWithArguments("MyName");
    /// </code>
    /// </example>
    /// <typeparam name="TEntity">
    /// The type of entity this <see cref="FactoryWithArguments{TEntity}"/> should create.
    /// </typeparam>
    /// <returns>Returns a new instance of <see cref="FactoryWithArguments{TEntity}"/>.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static FactoryWithArguments<TEntity> FactoryWithArguments<TEntity>()
        => new();
}
