using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestsSupport.Entities.Extensions;

/// <summary>
/// Extension methods to support fluent entity creation.
/// </summary>
public static partial class EntityExtensions
{
    /// <summary>
    /// Perform one or more configuration operation(s) on the provided <paramref name="entity"/>.
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
    /// <param name="entity">The <see cref="TEntity"/> to operate on.</param>
    /// <param name="configureDelegates">
    /// An array of actions that operates on the given <paramref name="entity"/>.
    /// </param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity With<TEntity>(this TEntity entity, params Action<TEntity>[] configureDelegates)
    {
        foreach (var configureDelegate in configureDelegates)
        {
            configureDelegate?.Invoke(entity);
        }

        return entity;
    }

    /// <summary>
    /// Perform an if statement.
    /// </summary>
    /// <example>
    /// Example how to use the if statement.
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> entity to perform the statement on.</param>
    /// <param name="condition">
    /// The condition that indicates whether the if or optionally the else should perform or not.
    /// </param>
    /// <param name="thenCase">The case to perform when the condition returns true.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity WithIf<TEntity>(this TEntity entity, bool condition, Action<TEntity> thenCase)
    {
        if (condition)
        {
            thenCase.Invoke(entity);
        }

        return entity;
    }

    /// <summary>
    /// Perform an if and else statement.
    /// </summary>
    /// <example>
    /// Example how to use the if with the else statement.
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> entity to perform the statement on.</param>
    /// <param name="condition">
    /// The condition that indicates whether the if or optionally the else should perform or not.
    /// </param>
    /// <param name="thenCase">The case to perform when the condition returns true.</param>
    /// <param name="elseCase">Optional case to perform when the condition returns false.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity WithIfElse<TEntity>(this TEntity entity, bool condition, Action<TEntity> thenCase,
        Action<TEntity> elseCase)
    {
        if (condition)
        {
            thenCase.Invoke(entity);
        }
        else
        {
            elseCase?.Invoke(entity);
        }

        return entity;
    }

    /// <summary>
    /// Perform an if statement.
    /// </summary>
    /// <example>
    /// Example how to use the if statement.
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> entity to perform the statement on.</param>
    /// <param name="condition">
    /// The condition that indicates whether the if case should perform or not.
    /// </param>
    /// <param name="thenCase">The case to perform when the condition returns true.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity WithIf<TEntity>(this TEntity entity, Func<TEntity, bool> condition, Action<TEntity> thenCase)
    {
        if (condition(entity))
        {
            thenCase.Invoke(entity);
        }

        return entity;
    }

    /// <summary>
    /// Perform an if and else statement.
    /// </summary>
    /// <example>
    /// Example how to use the if and else statement.
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> entity to perform the statement on.</param>
    /// <param name="condition">
    /// The condition that indicates whether the if or optionally the else should perform or not.
    /// </param>
    /// <param name="thenCase">The case to perform when the condition returns true.</param>
    /// <param name="elseCase">Optional case to perform when the condition returns false.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity WithIfElse<TEntity>(this TEntity entity, Func<TEntity, bool> condition, Action<TEntity> thenCase,
        Action<TEntity> elseCase = null)
    {
        if (condition(entity))
        {
            thenCase.Invoke(entity);
        }
        else
        {
            elseCase?.Invoke(entity);
        }

        return entity;
    }

    /// <summary>
    /// Attach the value returned by the <paramref name="createDelegate"/> to the given <paramref name="entity"/>.
    /// This will search all the properties and fields that conforms the type of <typeparamref name="TValue"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="configureDelegate"/> is only executed once, so if the value returned represents
    /// an collection, it won't get executed for each object that's added.
    /// </remarks>
    /// <example>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> to attach the returned value to.</param>
    /// <param name="createDelegate">An action that returns the value to attach.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <param name="configureDelegate">An optional configure action.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the value returned by <paramref name="configureDelegate"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when multiple properties and/or fields could be found that conforms the
    /// type returned by <paramref name="configureDelegate"/>.
    /// </exception>
    public static TEntity Attach<TEntity, TValue>(this TEntity entity,
        Func<TEntity, TValue> createDelegate,
        bool backLinkEnabled = true,
        Action<TEntity, TValue> configureDelegate = null) => entity.Attach(createDelegate.Invoke(entity), backLinkEnabled, configureDelegate);

    /// <summary>
    /// Attach the given <paramref name="value"/> to the given <paramref name="entity"/>.
    /// This will search all the properties and fields that conforms the type of <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// The <paramref name="configureDelegate"/> is only executed once, so if <paramref name="value"/> represents
    /// an collection, it won't get executed for each object that's added.
    /// </remarks>
    /// <example>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <param name="configureDelegate">An optional configure action.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the collection on <see cref="TEntity"/> is null or
    /// thrown when multiple properties and/or fields could be found that conforms the type of <paramref name="value"/>.
    /// </exception>
    public static TEntity Attach<TEntity, TValue>(this TEntity entity,
        TValue value,
        bool backLinkEnabled = true,
        Action<TEntity, TValue> configureDelegate = null)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var type = value.GetType();
        var isValueAGenericCollection = type.IsGenericCollection();
        var memberInfo = typeof(TEntity).GetSingleMemberInfoOrThrow(type, isValueAGenericCollection);
        if (memberInfo == null)
        {
            return entity;
        }

        // Set the value to the member if the member does not represents a generic collection.
        if (!memberInfo.ExecuteFunc(t => t.IsGenericCollection()))
        {
            memberInfo.SetValue(entity, value);

            if (backLinkEnabled)
            {
                entity.Backlink(value);
            }

            configureDelegate?.Invoke(entity, value);
            return entity;
        }

        // Get the value of the member and check if it's null or not. We try to help and new the collection,
        // only this creates issues when wanting to assert a specific behavior on a collection that is null. 
        var memberValue = GetValue(memberInfo, entity) ?? throw new InvalidOperationException(
            $"The member {memberInfo.Name} of {entity} is a " +
            "collection. Cannot attach to a collection that is null!");

        var isArray = memberInfo.ExecuteFunc(t => t.IsArray);

        if (isValueAGenericCollection)
        {
            // The given value is a generic collection, call AddRange.
            entity.AddRangeToCollection(value, memberInfo, memberValue, isArray, backLinkEnabled);
        }
        else
        {
            // The given value is a single object, call Add.
            entity.AddToCollection(value, memberInfo, memberValue, isArray, backLinkEnabled);
        }

        configureDelegate?.Invoke(entity, value);
        return entity;
    }

    /// <summary>
    /// Attach the given <paramref name="value"/> to the given <paramref name="entity"/> and use the given
    /// <paramref name="expression"/> to determine which property of field it should attach to.
    /// </summary>
    /// <example>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="expression">The expression to determine which property or field to attach to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <param name="configureDelegate">An optional configure action.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    public static TEntity AttachWith<TEntity, TValue>(this TEntity entity,
    Expression<Func<TEntity, TValue>> expression,
    TValue value,
    bool backLinkEnabled = true,
    Action<TEntity, TValue> configureDelegate = null)
    {
        var member = expression.Body as MemberExpression;
        member?.Member.SetValue(entity, value);

        if (backLinkEnabled)
        {
            entity.Backlink(value);
        }

        configureDelegate?.Invoke(entity, value);
        return entity;
    }

    /// <summary>
    /// Attach the given <paramref name="value"/> to a collection on the given <paramref name="entity"/>
    /// and use the given <paramref name="expression"/> to determine which property of field it should attach to.
    /// </summary>
    /// <example>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// -- redacted --
    /// <code>
    /// -- redacted --
    /// </code>
    /// </example>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="expression">The expression to determine which property or field to attach to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <param name="configureDelegate">An optional configure action.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the collection on <see cref="TEntity"/> is null.
    /// </exception>
    public static TEntity AttachWith<TEntity, TValue>(this TEntity entity,
        Expression<Func<TEntity, ICollection<TValue>>> expression,
        TValue value,
        bool backLinkEnabled = true,
        Action<TEntity, TValue> configureDelegate = null)
    {
        var func = expression.Compile();
        var collection = func.Invoke(entity);
        var member = expression.Body as MemberExpression;

        if (collection == null)
        {
            throw new InvalidOperationException($"The member {member?.Member.Name} of {entity} is a " +
                                                "collection. Cannot attach to a collection that is null!");
        }

        return entity.AddToCollection(collection, value, member?.Member, backLinkEnabled, configureDelegate);
    }

    /// <summary>
    /// Add the given <paramref name="value"/> to a member representing a collection on
    /// the given <paramref name="entity"/> that is defined in the <paramref name="memberInfo"/>.
    /// </summary>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="memberInfo">The property or field holding the collection.</param>
    /// <param name="memberValue">The actual value of the property or field.</param>
    /// <param name="isArray">Is the value of the property or field an array or not.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    private static void AddToCollection<TEntity, TValue>(this TEntity entity,
        TValue value,
        MemberInfo memberInfo,
        object memberValue,
        bool isArray,
        bool backLinkEnabled = true)
    {
        var list = ((IEnumerable<TValue>)memberValue).ToList();
        list.Add(value);

        if (backLinkEnabled)
        {
            entity.Backlink(value);
        }

        if (isArray)
        {
            memberInfo.SetValue(entity, list.ToArray());
        }
        else
        {
            memberInfo.SetValue(entity, list);
        }
    }

    /// <summary>
    /// Add the given <paramref name="value"/> to the given <paramref name="collection"/> on
    /// the given <paramref name="entity"/> that is defined in the <paramref name="memberInfo"/>.
    /// </summary>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="collection">The collection to add the value to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="memberInfo">The property or field holding the collection.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <param name="configureDelegate">An optional configure action.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    /// <returns>Returns the <paramref name="entity"/> instance to support fluent method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="collection"/> is null or
    /// thrown when <paramref name="value"/> is null or
    /// thrown when <paramref name="memberInfo"/> is null.
    /// </exception>
    private static TEntity AddToCollection<TEntity, TValue>(this TEntity entity,
        ICollection<TValue> collection,
        TValue value,
        MemberInfo memberInfo,
        bool backLinkEnabled,
        Action<TEntity, TValue> configureDelegate = null)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (memberInfo == null)
        {
            throw new ArgumentNullException(nameof(memberInfo));
        }

        var isArray = collection.GetType().IsArray;
        var list = collection.ToList();
        var type = value.GetType();
        var isValueAGenericCollection = type.IsGenericCollection();

        if (isValueAGenericCollection)
        {
            // The given value is a generic collection, call AddRange.
            entity.AddRangeToCollection(value, memberInfo, memberInfo.GetValue(entity), isArray, backLinkEnabled);
        }
        else
        {
            // The value is a single object, call Add.
            list.Add(value);

            if (backLinkEnabled)
            {
                entity.Backlink(value);
            }
        }

        if (isArray)
        {
            SetValue(memberInfo, entity, list.ToArray());
        }
        else
        {
            SetValue(memberInfo, entity, list);
        }

        configureDelegate?.Invoke(entity, value);
        return entity;
    }

    /// <summary>
    /// Adds the given <paramref name="value"/> to collection on the given <paramref name="entity"/>
    /// that is defined in the <paramref name="memberInfo"/>.
    /// </summary>
    /// <param name="entity">The <see cref="TEntity"/> to attach the <paramref name="value"/> to.</param>
    /// <param name="value">The <see cref="TValue"/> to attach.</param>
    /// <param name="memberInfo">The property or field holding the collection.</param>
    /// <param name="memberValue">The value of the property or field.</param>
    /// <param name="isArray">Whether the collection of the property or field is an array or not.</param>
    /// <param name="backLinkEnabled">Indicates whether the given value should backlink to the entity.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="memberValue"/> is null or
    /// thrown when <paramref name="value"/> is null or
    /// thrown when <paramref name="memberInfo"/> is null.
    /// </exception>
    private static void AddRangeToCollection<TEntity, TValue>(this TEntity entity,
        TValue value,
        MemberInfo memberInfo,
        object memberValue,
        bool isArray,
        bool backLinkEnabled)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (memberInfo == null)
        {
            throw new ArgumentNullException(nameof(memberInfo));
        }

        if (memberValue == null)
        {
            throw new ArgumentNullException(nameof(memberValue));
        }

        var genericType = memberInfo.ExecuteFunc(t => t.GetFirstOrDefaultGenericTypeArgumentOfGenericCollection());
        var func = genericType.CreateGenericListAndCallAddRange((IEnumerable<object>)memberValue);
        var values = ((IEnumerable<object>)value).ToList();
        var list = func.Invoke(values);

        if (backLinkEnabled)
        {
            values.ForEach(v => entity.Backlink(v));
        }

        if (isArray)
        {
            var array = ((IEnumerable<object>)list).ToArray();
            var destinationArray = Array.CreateInstance(genericType, array.Length);
            Array.Copy(array, destinationArray, array.Length);
            memberInfo.SetValue(entity, destinationArray);
        }
        else
        {
            memberInfo.SetValue(entity, list);
        }
    }

    /// <summary>
    /// Backlink the entity back to the value attached to the entity. 
    /// </summary>
    /// <param name="entity">The <see cref="TEntity"/> to attach back.</param>
    /// <param name="value">The value to attach on.</param>
    /// <typeparam name="TEntity">The type of the <paramref name="entity"/>.</typeparam>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="entity"/> is null or
    /// thrown when <paramref name="value"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the collection on <see cref="value"/> is null.
    /// </exception>
    private static void Backlink<TEntity>(this TEntity entity, object value)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var type = typeof(TEntity);
        var isValueAGenericCollection = type.IsGenericCollection();
        var memberInfo = value.GetType().GetSingleMemberInfoOrThrow(type, isValueAGenericCollection);
        if (memberInfo == null)
        {
            return;
        }

        // Set the value to the member if the member does not represents a generic collection.
        if (!memberInfo.ExecuteFunc(t => t.IsGenericCollection()))
        {
            memberInfo.SetValue(value, entity);
            return;
        }

        // Get the value of the member and check if it's null or not. We try to help and new the collection,
        // only this creates issues when wanting to assert a specific behavior on a collection that is null. 
        var memberValue = GetValue(memberInfo, value) ?? throw new InvalidOperationException(
            $"The member {memberInfo.Name} of {value} is a " +
            "collection. Cannot attach to a collection that is null!");

        var isArray = memberInfo.ExecuteFunc(t => t.IsArray);

        if (isValueAGenericCollection)
        {
            // The given value is a generic collection, call AddRange.
            value.AddRangeToCollection(entity, memberInfo, memberValue, isArray, backLinkEnabled: false);
        }
        else
        {
            // The given value is a single object, call Add.
            value.AddToCollection(entity, memberInfo, memberValue, isArray, backLinkEnabled: false);
        }
    }

    /// <summary>
    /// Set the given <paramref name="value"/> on the given <paramref name="entity"/> property or field. 
    /// </summary>
    /// <param name="memberInfo">The property or field.</param>
    /// <param name="entity">The object having the property or field.</param>
    /// <param name="value">The value to set.</param>
    private static void SetValue(this MemberInfo memberInfo, object entity, object value)
    {
        switch (memberInfo)
        {
            case PropertyInfo propertyInfo:
                propertyInfo.SetValue(entity, value);
                break;
            case FieldInfo fieldInfo:
                fieldInfo.SetValue(entity, value);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Get the value of the given <paramref name="entity"/> property or field.
    /// </summary>
    /// <param name="memberInfo">The property or field.</param>
    /// <param name="entity">The object having the property or field.</param>
    /// <returns>The retrieved value.</returns>
    private static object GetValue(this MemberInfo memberInfo, object entity)
    {
        return memberInfo switch
        {
            PropertyInfo propertyInfo => propertyInfo.GetValue(entity),
            FieldInfo fieldInfo => fieldInfo.GetValue(entity),
            _ => null
        };
    }

    /// <summary>
    /// Execute the given <paramref name="func"/> on the given <paramref name="memberInfo"/>.
    /// </summary>
    /// <param name="memberInfo">The property or field.</param>
    /// <param name="func">The func to invoke.</param>
    /// <typeparam name="T">The <see cref="Type"/> of the argument to return.</typeparam>
    /// <returns>Returns the retrieved value from the invoked func.</returns>
    private static T ExecuteFunc<T>(this MemberInfo memberInfo, Func<Type, T> func)
    {
        return memberInfo switch
        {
            PropertyInfo propertyInfo => func.Invoke(propertyInfo.PropertyType),
            FieldInfo fieldInfo => func.Invoke(fieldInfo.FieldType),
            _ => default
        };
    }

    /// <summary>
    /// Get a single <see cref="MemberInfo"/> or throw when multiple could be found.
    /// </summary>
    /// <param name="entityType">
    /// The type of the entity that contains the <see cref="PropertyInfo"/> or <see cref="FieldInfo"/>.
    /// </param>
    /// <param name="type">The type of the <see cref="PropertyInfo"/> or <see cref="FieldInfo"/> itself.</param>
    /// <param name="isValueAGenericCollection">Indicates whether the value is a generic collection or not.</param>
    /// <returns>Returns a single <see cref="MemberInfo"/> if one could be found; otherwise the default value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there are multiple properties or fields found of type <paramref name="type"/>.
    /// </exception>
    private static MemberInfo GetSingleMemberInfoOrThrow(this Type entityType, Type type, bool isValueAGenericCollection)
    {
        var typeToSearch = isValueAGenericCollection
            ? type.GetFirstOrDefaultGenericTypeArgumentOfGenericCollection()
            : type;

        bool Filter(Type memberType, Type valueType)
        {
            return memberType == valueType || memberType.IsGenericCollectionOfType(typeToSearch);
        }

        var propertyInfo = entityType.GetSingleOrDefaultPropertyInfoOrThrow(type, Filter);
        var fieldInfo = entityType.GetSingleOrDefaultFieldInfoOrThrow(type, Filter);

        if (propertyInfo != null && fieldInfo != null)
        {
            throw new InvalidOperationException($"Found a property and field with type {type} " +
                                                $"on {entityType} use the '{nameof(AttachWith)}' method to " +
                                                "specify the property or field.");
        }

        return propertyInfo != null ? propertyInfo : fieldInfo;
    }

    /// <summary>
    /// Get a single <see cref="PropertyInfo"/> or throw when multiple could be found.
    /// </summary>
    /// <param name="entityType">The type of the entity that contains the <see cref="PropertyInfo"/>.</param>
    /// <param name="type">The type of the <see cref="PropertyInfo"/> itself.</param>
    /// <param name="filter">Filter to find the <see cref="PropertyInfo"/> with.</param>
    /// <returns>
    /// Returns a single <see cref="PropertyInfo"/> if one could be found; otherwise the default value.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there are multiple properties found of type <paramref name="type"/>.
    /// </exception>
    private static PropertyInfo GetSingleOrDefaultPropertyInfoOrThrow(this Type entityType, Type type, Func<Type, Type, bool> filter)
    {
        var properties = entityType.GetProperties()
            .Where(p => filter.Invoke(p.PropertyType, type))
            .ToArray();

        if (properties.Length > 1)
        {
            throw new InvalidOperationException($"Multiple properties with type {type} found on {entityType}, " +
                                                $"use the '{nameof(AttachWith)}' method to specify the property.");
        }

        return properties.SingleOrDefault();
    }

    /// <summary>
    /// Get a single <see cref="FieldInfo"/> or throw when multiple could be found.
    /// </summary>
    /// <param name="entityType">The type of the entity that contains the <see cref="FieldInfo"/>.</param>
    /// <param name="type">The type of the <see cref="FieldInfo"/> itself.</param>
    /// <param name="filter">Filter to find the <see cref="FieldInfo"/> with.</param>
    /// <returns>
    /// Returns a single <see cref="FieldInfo"/> if one could be found; otherwise the default value.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there are multiple fields found of type <paramref name="type"/>.
    /// </exception>
    private static FieldInfo GetSingleOrDefaultFieldInfoOrThrow(this Type entityType, Type type, Func<Type, Type, bool> filter)
    {
        var fields = entityType.GetFields()
            .Where(f => filter.Invoke(f.FieldType, type))
            .ToArray();

        if (fields.Length > 1)
        {
            throw new InvalidOperationException($"Multiple fields with type {type} found on {entityType}, " +
                                                $"use the '{nameof(AttachWith)}' method to specify the field.");
        }

        return fields.SingleOrDefault();
    }

    /// <summary>
    /// Is the <paramref name="type"/> a generic collection.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns>Returns true if the <paramref name="type"/> is a generic collection; otherwise false.</returns>
    private static bool IsGenericCollection(this Type type)
    {
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
               type.GetInterfaces().Any(i => i.IsGenericCollection());
    }

    /// <summary>
    /// Is the <paramref name="type"/> a generic collection of given <paramref name="argumentType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <param name="argumentType">The <see cref="Type"/> of the generic type argument.</param>
    /// <returns>
    /// Returns true if the <paramref name="type"/> is a generic collection of given <paramref name="argumentType"/>; otherwise false.
    /// </returns>
    private static bool IsGenericCollectionOfType(this Type type, Type argumentType)
    {
        return type.IsGenericCollection() && (type.GenericTypeArguments.FirstOrDefault() == argumentType ||
                                       type.GetInterfaces().Any(i => i.GenericTypeArguments.FirstOrDefault() == argumentType));
    }

    /// <summary>
    /// Get the generic type argument of a generic collection.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the generic type argument from.</param>
    /// <returns>Returns the first found generic type argument.</returns>
    private static Type GetFirstOrDefaultGenericTypeArgumentOfGenericCollection(this Type type)
    {
        return type.GenericTypeArguments.FirstOrDefault() ?? type.GetInterfaces()
            .Where(i => i.IsGenericType)
            .SelectMany(i => i.GenericTypeArguments)
            .FirstOrDefault();
    }

    /// <summary>
    /// Creates a new generic list of type <paramref name="genericType"/> and pass in the original collection values.
    /// </summary>
    /// <param name="genericType">The generic type that the list should have.</param>
    /// <param name="originalCollection">The original collection of the Field/Property.</param>
    /// <returns>Returns a new Func with a collection argument and returns the newly created list.</returns>
    private static Func<IEnumerable<object>, object> CreateGenericListAndCallAddRange(this Type genericType,
        IEnumerable<object> originalCollection)
    {
        // Get the generic type of the list.
        var genericListType = typeof(List<>).MakeGenericType(genericType);

        // Create an instance of the generic list and pass in the original collection values from the Property or Field.
        var list = originalCollection != null
            ? Activator.CreateInstance(genericListType, originalCollection)
            : Activator.CreateInstance(genericListType);

        // Create a variable of the generic list.
        var instance = Expression.Variable(genericListType, "list");

        // Assign the list object to the variable.
        var assign = Expression.Assign(instance, Expression.Constant(list));

        // Create a variable of the collection that should be passed in to the AddRange call.
        var values = Expression.Parameter(typeof(IEnumerable<object>), "values");

        // Create a Cast (.Cast<>) method, so that we can call the AddRange method to it.
        var castCall = genericType.CreateCastMethodCallOfGenericType(values);

        // Create the AddRange call.
        var addRangeCall = genericType.CreateAddRangeMethodCallOfGenericType(genericListType, instance, castCall);

        // Create a Block expression that represents the actual actions to perform.
        var block = Expression.Block([instance], assign, addRangeCall, instance);

        // Create a Lambda expression that returns a Func with a collection argument and returns a list.
        return (Func<IEnumerable<object>, object>)Expression.Lambda(block, values).Compile();
    }

    /// <summary>
    /// Create a Cast (.Cast{}) method, so that we can call the AddRange method to it.
    /// </summary>
    /// <param name="genericType">The generic type.</param>
    /// <param name="values">The expression that represents a collection.</param>
    /// <returns>Returns a new <see cref="MethodCallExpression"/>.</returns>
    private static MethodCallExpression CreateCastMethodCallOfGenericType(this Type genericType, Expression values)
    {
        var castMethod = typeof(Enumerable).GetMethod("Cast", [typeof(IEnumerable)]);
        Debug.Assert(castMethod != null, nameof(castMethod) + " != null");
        return Expression.Call(castMethod.MakeGenericMethod(genericType), values);
    }

    /// <summary>
    /// Create the AddRange call.
    /// </summary>
    /// <param name="genericType">The generic type.</param>
    /// <param name="genericListType">The type of the generic list.</param>
    /// <param name="instance">The generic list variable.</param>
    /// <param name="castCall">The Cast call.</param>
    /// <returns>Returns a new <see cref="MethodCallExpression"/>.</returns>
    private static MethodCallExpression CreateAddRangeMethodCallOfGenericType(this Type genericType,
        Type genericListType,
        Expression instance,
        Expression castCall)
    {
        var addRangeMethod = genericListType.GetMethod("AddRange",
            [typeof(IEnumerable<>).MakeGenericType(genericType)]);
        Debug.Assert(addRangeMethod != null, nameof(addRangeMethod) + " != null");
        return Expression.Call(instance, addRangeMethod, castCall);
    }
}
