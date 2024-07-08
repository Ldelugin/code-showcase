using System;
using System.Collections.Generic;
using System.Linq;

namespace TestsSupport.TestClasses;

/// <summary>
/// Represents a parent-child relationship between objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of objects in the parent-child relationship.</typeparam>
public sealed class ParentChildRelationship<T> : IDisposable where T : IDisposable
{
    /// <summary>
    /// Gets or sets the ID of the parent-child relationship.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the list of items associated with the parent-child relationship.
    /// </summary>
    public List<T> List { get; } = [];

    /// <summary>
    /// Gets the parent relationship of the current parent-child relationship.
    /// </summary>
    public ParentChildRelationship<T> Parent { get; private set; }

    /// <summary>
    /// Gets the dictionary of child relationships categorized by their associated type.
    /// </summary>
    public Dictionary<Type, HashSet<ParentChildRelationship<T>>> Children { get; } = [];

    /// <summary>
    /// Adds a child relationship to the current parent-child relationship.
    /// </summary>
    /// <param name="type">The type associated with the child relationship.</param>
    /// <param name="childRelationship">The child relationship to add.</param>
    /// <returns><c>true</c> if the child relationship was added successfully; otherwise, <c>false</c>.</returns>
    public bool AddChild(Type type, ParentChildRelationship<T> childRelationship)
    {
        childRelationship.Parent = this;
        // ReSharper disable once InvertIf
        if (!this.Children.TryGetValue(type, out var childrenSet))
        {
            childrenSet = [];
            this.Children.Add(type, childrenSet);
        }

        return childrenSet.Add(childRelationship);
    }

    /// <summary>
    /// Removes a child relationship from the current parent-child relationship.
    /// </summary>
    public void Dispose()
    {
        this.List.ForEach(item => item.Dispose());
        this.Children.SelectMany(child => child.Value).ToList().ForEach(child => child.Dispose());
        this.Children.Clear();
        this.List.Clear();
        this.Parent = null;
    }
}