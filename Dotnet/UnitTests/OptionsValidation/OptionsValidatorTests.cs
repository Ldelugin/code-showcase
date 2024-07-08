using System;
using System.Collections.Generic;
using System.Globalization;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSupport;
using TestsSupport.Extensions;

namespace -- redacted --;

[TestClass]
public class OptionsValidatorTests
{
    private readonly OptionsValidator optionsValidator = new();

    public static IEnumerable<object[]> Get-- redacted --OptionsDefaultValues()
    {
        var -- redacted -- = new -- redacted --();
        -- redacted -- = OptionInstantiator.InitializeOptionsInstance(-- redacted --);

        var objectList = new List<object[]>();
        CreateNodeValues(-- redacted --, objectList, string.Empty);
        foreach (var p in objectList)
        {
            yield return p;
        }
    }

    public static IEnumerable<object[]> Get-- redacted --OptionsDefaultValues()
    {
        var -- redacted -- = new -- redacted --();
        -- redacted -- = OptionInstantiator.InitializeOptionsInstance(-- redacted --);

        var objectList = new List<object[]>();
        CreateNodeValues(-- redacted --, objectList, $"{-- redacted --}:{Guid.NewGuid()}:");
        foreach (var p in objectList)
        {
            yield return p;
        }
    }

    private static void CreateNodeValues(object optionsObject, List<object[]> objectList, string fullKey)
    {
        var allPropertyInfos = optionsObject.GetType().GetProperties();
        fullKey = fullKey + optionsObject.GetType().Name + ':';
        foreach (var optionsProperty in allPropertyInfos)
        {
            var newOptionsObject = optionsProperty.GetValue(optionsObject);
            if (optionsProperty.IsDefined(typeof(IsNestedAttribute), inherit: false) ||
                optionsProperty.IsDefined(typeof(IsNamedAttribute), inherit: false))
            {
                CreateNodeValues(newOptionsObject, objectList, fullKey);
            }
            else
            {
                var newFullKey = fullKey + optionsProperty.Name;
                objectList.Add(
                [
                    newFullKey,
                    optionsProperty.GetValue(optionsObject) == null
                        ? string.Empty
                        : Convert.ToString(optionsProperty.GetValue(optionsObject), CultureInfo.InvariantCulture),
                    false
                ]);
            }
        }
    }

    public static IEnumerable<object[]> GetNonDefaultValues()
    {
        var optionValuesToAssert = new List<AssertableOptionValuesBase>();
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);
        optionValuesToAssert.AddRange(-- redacted --);

        foreach (var assertableOptionValues in optionValuesToAssert)
        {
            var key = assertableOptionValues.Key;
            foreach (var (value, expectedToFail) in assertableOptionValues.Arguments)
            {
                yield return new object[] { key, value, expectedToFail };
            }
        }
    }

    public static IEnumerable<object[]> GetNonDefaultCompoundValues()
    {
        var compoundOptionValuesToAssert = new List<AssertableCompoundOptionValuesBase>();
        compoundOptionValuesToAssert.AddRange(CompoundRouterOptionsValues);

        foreach (var assertableCompoundOptionValues in compoundOptionValuesToAssert)
        {
            var firstKey = assertableCompoundOptionValues.FirstKey;
            var secondKey = assertableCompoundOptionValues.SecondKey;

            foreach (var (firstValue, secondValue, expectedToFail) in assertableCompoundOptionValues.Arguments)
            {
                yield return new object[] { firstKey, firstValue, secondKey, secondValue, expectedToFail };
            }
        }
    }

    -- redacted --

    private static readonly List<AssertableOptionValuesBase> -- redacted -- =
    [
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert(@"domain", expectedToFail: true)
            .Assert(@"domain/username", expectedToFail: true)
            .Assert(@"domain\username;", expectedToFail: true)
            .Assert(@"domain*\username", expectedToFail: true)
            .Assert(@"domain\username*", expectedToFail: true)
            .Assert(@"domain\username:domain\username", expectedToFail: true)
            .Assert(@"domain\username;domain\username", expectedToFail: false)
            .Assert(@"domain\username", expectedToFail: false)
            .Assert(@"domain.com\username.lastname", expectedToFail: false)
    ];

    private static readonly List<AssertableOptionValuesBase> -- redacted -- =
    [
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: false)
            .Assert("-1", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("5", expectedToFail: false),
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: false)
            .Assert("-1", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("5", expectedToFail: false)
    ];

    private static readonly List<AssertableOptionValuesBase> -- redacted -- =
    [
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertUrl(alsoAssertWithPort: true, ["http", "https"]),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertNotRequired()
            .AssertPassword(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: true)
            .Assert("-10", expectedToFail: true)
            .Assert("251", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("250", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertRequired()
            .AssertSoapEndpoint(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertRequired()
            .AssertSoapEndpoint(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertRequired()
            .AssertSoapEndpoint(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("-00:00:01", expectedToFail: true)
            .Assert("00:01:00", expectedToFail: false)
            .Assert("00:00:00", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: true)
            .Assert("-10", expectedToFail: true)
            .Assert("251", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("250", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("-00:00:01", expectedToFail: true)
            .Assert("00:01:00", expectedToFail: false)
            .Assert("00:00:00", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: true)
            .Assert("-10", expectedToFail: true)
            .Assert("251", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("250", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertUrl(alsoAssertWithPort: true, ["http", "https"]),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertRequired(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .AssertRequired(),
        new AssertableOptionValues<-- redacted --, -- redacted -->(-- redacted --)
            .Assert("default,", expectedToFail: true)
            .Assert("default;super-user", expectedToFail: true)
            .Assert("default", expectedToFail: false)
            .Assert("default,super-user", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted --, -- redacted -->(-- redacted --)
            .Assert("-00:00:01", expectedToFail: true)
            .Assert("00:01:00", expectedToFail: false)
            .Assert("00:00:00", expectedToFail: false),
        new AssertableOptionValues<-- redacted --, -- redacted --, -- redacted -->(-- redacted --)
            .Assert("0", expectedToFail: true)
            .Assert("-10", expectedToFail: true)
            .Assert("251", expectedToFail: true)
            .Assert("1", expectedToFail: false)
            .Assert("250", expectedToFail: false),
    ];

    private static readonly List<AssertableOptionValuesBase> -- redacted -- =
    [
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .AssertValidServiceWorkerConfiguration(),
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .AssertValidServiceWorkerConfiguration(),
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .AssertValidServiceWorkerConfiguration(),
        new AssertableOptionValues<-- redacted -->(-- redacted --)
            .AssertValidServiceWorkerConfiguration()
    ];

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    -- redacted --

    [TestMethod]
    [DynamicData(nameof(Get-- redacted --OptionsDefaultValues), DynamicDataSourceType.Method)]
    public void ValidateAllDefault-- redacted --OptionsValues(string key, string value, bool expectedToFail)
        => this.ValidateOptions<-- redacted -->(key, value, expectedToFail);

    [TestMethod]
    [DynamicData(nameof(Get-- redacted --OptionsDefaultValues), DynamicDataSourceType.Method)]
    public void ValidateAll-- redacted --OptionsValues(string key, string value, bool expectedToFail)
        => this.ValidateOptions<-- redacted -->(key, value, expectedToFail);

    [TestMethod]
    [DynamicData(nameof(GetNonDefaultValues), DynamicDataSourceType.Method)]
    public void ValidateNonDefaultValues(string key, string value, bool expectedToFail)
        => this.ValidateOptions<-- redacted -->(key, value, expectedToFail);

    [TestMethod]
    [DynamicData(nameof(GetNonDefaultCompoundValues), DynamicDataSourceType.Method)]
    public void ValidateNonDefaultCompoundValues(string firstKey,
        string firstValue,
        string secondKey,
        string secondValue,
        bool expectedToFail)
        => this.ValidateCompoundOptions(firstKey, firstValue, secondKey, secondValue, expectedToFail);

    private void ValidateOptions<T>(string key, string value, bool expectedToFail) where T : class, new()
    {
        // Act
        var result = this.optionsValidator.ValidateByDataAnnotation<T>(key, value);

        // Assert
        Assert.AreEqual(expectedToFail, result.Failed, result.FailureMessage);
        Assert.AreNotEqual(expectedToFail, result.Succeeded);
        Assert.IsFalse(result.Skipped);
    }

    private void ValidateCompoundOptions(string firstKey, string firstValue, string secondKey,
        string secondValue, bool expectedToFail)
    {
        var -- redacted -- = new -- redacted --();
        -- redacted -- = OptionInstantiator.InitializeOptionsInstance(-- redacted --);
        -- redacted --.Set-- redacted --OptionsValues(firstKey, firstValue);
        -- redacted --.Set-- redacted --OptionsValues(secondKey, secondValue);

        this.ValidateOptions<-- redacted -->(firstKey, firstValue, expectedToFail);
    }
}
