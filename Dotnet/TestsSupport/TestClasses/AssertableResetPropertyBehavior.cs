using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestsSupport.Extensions;

namespace TestsSupport.TestClasses;

/// <summary>
/// This class provides methods to assert the reset property behavior of specified classes.
/// It offers a way to verify the reset behavior of different types of objects (-- redacted --)
/// through its Verify() method. 
/// </summary>
public sealed class AssertableResetPropertyBehavior : IDisposable
{
    private static readonly Dictionary<Type, Dictionary<string, Action<object, TestMock<-- redacted -->>>> ResetMethodAsserts = new()
    {
        { typeof(-- redacted --), new Dictionary<string, Action<object, TestMock<-- redacted -->>>
            {
                { nameof(-- redacted --.-- redacted --), -- redacted -- }
            }
        },
        { typeof(-- redacted --), new Dictionary<string, Action<object, TestMock<-- redacted -->>>
            {
                { nameof(-- redacted --.-- redacted --), -- redacted -- }
            }
        },
        { typeof(-- redacted --), new Dictionary<string, Action<object, TestMock<-- redacted -->>>
            {
                { nameof(-- redacted --.-- redacted --), -- redacted -- }
            }
        }
    };

    private static readonly Dictionary<Type, Dictionary<string, Func<object, object, bool>>> NewValueMethodAsserts = new()
    {
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>
            {
                { nameof(-- redacted --.-- redacted --), -- redacted -- }
            }
        },
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>
            {
                { nameof(Image.-- redacted --), -- redacted -- }
            }
        },
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>() }
    };

    private static readonly Dictionary<Type, Dictionary<string, Func<object, object, bool>>> IndirectlyMethodAsserts = new()
    {
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>
        {
            -- redacted --
        } },
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>
        {
            -- redacted --
        } },
        { typeof(-- redacted --), new Dictionary<string, Func<object, object, bool>>() }
    };

    public static bool DatabaseMode { get; set; }

    /// <summary>
    /// Initializes a new instance of the `AssertableResetPropertyBehavior` class.
    /// This constructor will initialize the properties with specified `ISupportsReset` instance and `PropertyInfo`.
    /// </summary>
    /// <param name="propertyInfo">The property information.</param>
    /// <param name="instance">The instance supporting reset.</param>
    public AssertableResetPropertyBehavior(PropertyInfo propertyInfo, ISupportsReset instance)
    {
        this.PropertyInfo = propertyInfo;
        this.Instance = instance;
        this.ResetPropertyBehaviorAttribute = this.PropertyInfo.GetCustomAttribute<ResetPropertyBehaviorAttribute>();
        this.ValueBeforeReset = this.PropertyInfo.GetValue(instance);
        if (this.ValueBeforeReset?.GetType() != typeof(-- redacted --) || DatabaseMode)
        {
            return;
        }

        this.-- redacted -- = new TestMock<-- redacted -->(this.ValueBeforeReset as -- redacted --)
        {
            CallBase = true
        };
        // so that we can verify it called the correct method
        this.PropertyInfo.SetValue(instance, this.-- redacted --.Object);
    }

    private ISupportsReset Instance { get; set; }
    private PropertyInfo PropertyInfo { get; set; }
    private object ValueBeforeReset { get; }
    private ResetPropertyBehaviorAttribute ResetPropertyBehaviorAttribute { get; set; }
    private TestMock<-- redacted --> -- redacted -- { get; }

    /// <summary>
    /// Sync the instance and property info with the specified `ISupportsReset` instance and `PropertyInfo`.
    /// </summary>
    /// <param name="instance">Instance of <see cref="ISupportsReset"/>.</param>
    public void Sync(ISupportsReset instance)
    {
        this.Instance = instance;
        this.PropertyInfo = this.Instance.GetType().GetProperty(this.PropertyInfo.Name);
        this.ResetPropertyBehaviorAttribute = this.PropertyInfo?.GetCustomAttribute<ResetPropertyBehaviorAttribute>();
    }

    /// <summary>
    /// Dispose the `-- redacted --` if it is not null.
    /// </summary>
    public void Dispose() => this.-- redacted --?.Dispose();

    /// <summary>
    /// Verify the reset property behavior according to the rules and logic defined for the instance.
    /// </summary>
    public void Verify()
    {
        var currentValue = this.PropertyInfo.GetValue(this.Instance);
        try
        {
            switch (this.ResetPropertyBehaviorAttribute.ResetMode)
            {
                case ResetMode.DoNotReset:
                    Assert.IsTrue(AreObjectsEqual(this.ValueBeforeReset, currentValue));
                    break;
                case ResetMode.Indirectly:
                    if (DatabaseMode)
                    {
                        // Indirectly will change, but how is different per thing, so maybe add something for that?
                        var assert = IndirectlyMethodAsserts[this.Instance.GetType()][this.PropertyInfo.Name];
                        Assert.IsTrue(assert(this.ValueBeforeReset, currentValue));
                        break;
                    }

                    Assert.IsTrue(AreObjectsEqual(this.ValueBeforeReset, currentValue));
                    break;
                case ResetMode.DefaultValue:
                    Assert.IsTrue(this.ResetPropertyBehaviorAttribute.DefaultValue != null
                        ? AreObjectsEqual(currentValue, this.ResetPropertyBehaviorAttribute.DefaultValue)
                        : currentValue.IsNullOrDefault());
                    break;
                case ResetMode.NewValue:
                    var newValue = Activator.CreateInstance(this.PropertyInfo.PropertyType);
                    var newValueAssert = NewValueMethodAsserts[this.Instance.GetType()][this.PropertyInfo.Name];
                    Assert.IsTrue(newValueAssert?.Invoke(currentValue, newValue));
                    break;
                case ResetMode.ResetMethod:
                    var resetMethodAssert = ResetMethodAsserts[this.Instance.GetType()][this.PropertyInfo.Name];
                    resetMethodAssert?.Invoke(currentValue, this.-- redacted --);
                    break;
                case ResetMode.IncrementCounter:
                    Assert.AreEqual(((int)this.ValueBeforeReset) + 1, currentValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(this.ResetPropertyBehaviorAttribute.ResetMode),
                        this.ResetPropertyBehaviorAttribute.ResetMode, @"Invalid value used");
            }
        }
        finally
        {
            this.-- redacted --?.Dispose();
        }
    }

    private static bool AreObjectsEqual(object left, object right)
    {
        switch (left)
        {
            case null:
                return right is null;
            case ICollection collectionLeft when right is ICollection collectionRight:
                CollectionAssert.AreEqual(collectionLeft, collectionRight);
                return true;
            case DateTime dateTimeLeft when right is DateTime dateTimeRight:
                return dateTimeLeft.Trim(TimeSpan.FromSeconds(value: 1)) == dateTimeRight.Trim(TimeSpan.FromSeconds(value: 1));
            case -- redacted -- when right is -- redacted --:
                return -- redacted --.Id == -- redacted --.Id;
            default:
                return left.Equals(right);
        }
    }

    private static bool AreObjectsNotEqual(object left, object right)
    {
        switch (left)
        {
            case null:
                return right is not null;
            case ICollection collectionLeft when right is ICollection collectionRight:
                CollectionAssert.AreNotEqual(collectionLeft, collectionRight);
                return true;
            case DateTime dateTimeLeft when right is DateTime dateTimeRight:
                return dateTimeLeft.Trim(TimeSpan.FromSeconds(value: 1)) != dateTimeRight.Trim(TimeSpan.FromSeconds(value: 1));
            default:
                return !left.Equals(right);
        }
    }

    private static void -- redacted --(object instance, TestMock<-- redacted --> mock)
    {
        var -- redacted -- = instance as -- redacted --;
        if (-- redacted -- == null)
        {
            Assert.Fail("Can't assert an object that is either null or not a -- redacted --.");
        }

        -- redacted --(-- redacted --);

        // As the instance is different when dealing with the database, the next step is impossible to verify.
        if (DatabaseMode)
        {
            return;
        }

        try
        {
            mock.Verify(m => m.ResetBackToHostOnly(), Times.Once);
        }
        finally
        {
            mock?.Dispose();
        }
    }

    private static void -- redacted --(-- redacted --)
    {
        var -- redacted -- = -- redacted --.ToList(-- redacted --, includeDuplicates: true);
        var actual = -- redacted --.Count(-- redacted --);
        Assert.AreEqual(expected: 0, actual: actual);
    }

    private static bool -- redacted --(object value, object newValue)
    {
        var left = value as -- redacted --;
        var right = newValue as -- redacted --;
        return AreObjectsEqual(left?.Serialized, right?.Serialized);
    }

    private static bool AssertCollectionIsReset<T>(object valueBeforeReset, object currentValue)
    {
        var left = valueBeforeReset as ICollection<T>;
        // If left is null or has no elements, it means that the collection was empty before the reset.
        // No need to assert anything.
        if (left?.Count == 0)
        {
            return true;
        }

        var right = currentValue as ICollection<T>;
        Assert.AreNotEqual(left?.Count, right?.Count);
        Assert.AreEqual(expected: 0, right?.Count ?? 0);
        return AreObjectsNotEqual(left, right);
    }

    private static bool -- redacted --(object valueBeforeReset, object currentValue)
    {
        var left = valueBeforeReset as ICollection<-- redacted -->;
        var right = currentValue as ICollection<-- redacted -->;
        var -- redacted -- = left?.Count(-- redacted --);
        // -- redacted --
        // No need to assert anything.
        if (-- redacted -- == 0)
        {
            return true;
        }
        var -- redacted -- = right?.Count(-- redacted --);
        return AreObjectsNotEqual(-- redacted --, -- redacted --);
    }

    private static bool -- redacted --(object valueBeforeReset, object currentValue)
    {
        var left = valueBeforeReset as ICollection<-- redacted -->;
        var right = currentValue as ICollection<-- redacted -->;
        Assert.AreNotEqual(left?.Count, right?.Count);
        Assert.AreEqual(left?.Count + 1, right?.Count);
        return AreObjectsNotEqual(left, right);
    }

    private static bool -- redacted --(object valueBeforeReset, object currentValue) =>
        AreObjectsNotEqual(valueBeforeReset, currentValue);

    private static bool -- redacted --(object valueBeforeReset, object currentValue)
    {
        var left = valueBeforeReset as ICollection<-- redacted -->;
        var right = currentValue as ICollection<-- redacted -->;
        Assert.AreNotEqual(left?.Count, right?.Count);
        return AreObjectsNotEqual(left, right) &&
               right?.All(-- redacted --) == true;
    }

    private static bool AssertRowVersionIncremented(object valueBeforeReset, object currentValue)
    {
        var left = valueBeforeReset as byte[];
        var right = currentValue as byte[];
        return AreObjectsNotEqual(left, right);
    }

    -- redacted --
}