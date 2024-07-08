using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using -- redacted --;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using -- redacted --;
using -- redacted --;
using TestsSupport.Entities;
using TestsSupport.Entities.Extensions;
using TestsSupport.Entities.Extensions.-- redacted --;
using TestsSupport.Soap;
using TestsSupport.Soap.Extensions;

namespace -- redacted --;

// Somehow the test runner is not able to handle the following inside a DynamicData method.
// The issue is that the request object is created and also recognized for example that the Passages property contains 1 element
// only when it passes as parameter the element is newed instead of the one created in the method.
// So to get around this issue, I do a trick with creating them in a different method and get the index from the DataRow attribute.
// https://github.com/microsoft/testfx/issues/1462
[TestClass]
public class ValidationTests : ValidationTestsBase
{
    [ClassInitialize]
    public static async Task ClassInitialize(TestContext _1)
    {
        WebApplicationFactory = new -- redacted --<Startup>();
        await WebApplicationFactory.AddUserAsync(UserName, UserName, Password);
        await WebApplicationFactory.AddAllClaimsToUser(UserName);
    }

    private static Type ServiceType => typeof(-- redacted --);
    protected override SoapVersion SoapVersion => SoapVersion.Soap12;
    protected override string RelativeRequestUri => "-- redacted --";

    [TestCleanup]
    public override void Cleanup() => base.Cleanup();

    private static IEnumerable<object[]> GetRequestObjectsForStructureValidation()
    {
        yield return new object[] { Entity.-- redacted --.Create() };
        yield return new object[] { Entity.-- redacted --.Create() };
        yield return new object[] { Entity.-- redacted --.Create() };
        yield return new object[] { Entity.-- redacted --.Create() };
    }

    [TestMethod]
    [DynamicData(nameof(GetRequestObjectsForStructureValidation), DynamicDataSourceType.Method)]
    public async Task FailsStructureValidation(object requestObject)
    {
        // Arrange
        var settings = SoapParserSettings.Soap12.WithOmitBodyContent();
        var soapRequest = requestObject.AsSoap(settings);
        this.TestContext.WriteLine($"Soap request: {soapRequest}");

        // Act / Assert
        await this.AssertStructureValidationErrorAsync(soapRequest);
    }

    private static List<(object, SoapParserSettings)> -- redacted --RequestData()
    {
        return
        [
            ( // <ser:MaxResults>abc</ser:MaxResults>
                Entity.-- redacted --.Default(),
                SoapParserSettings.Soap12
                    .WithOverride(new XPathWithValue("//ser:-- redacted --", "abc"))
            ),
            ( // <ser:GetResults />
                Entity.-- redacted --.Default(), SoapParserSettings.Soap12.WithPartiallyOmitBodyContent()
            )
        ];
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public async Task FailsSchemaValidationFor-- redacted --Request(int index)
    {
        // Arrange
        var (requestObject, settings) = -- redacted --()[index];

        // Act / Assert
        await this.AssertSchemaValidationErrorAsync(requestObject, settings, ServiceType);
    }

    private static List<(object, SoapParserSettings)> -- redacted --RequestData()
    {
        return
        [
            (Entity.-- redacted --.Create(), null),
        ];
    }

    [TestMethod]
    [DataRow(0)]
    public async Task FailsSchemaValidationFor-- redacted --Request(int index)
    {
        // Arrange
        var (requestObject, settings) = -- redacted --()[index];

        // Act / Assert
        await this.AssertSchemaValidationErrorAsync(requestObject, settings, ServiceType);
    }

    private static List<(object, SoapParserSettings)> -- redacted --RequestData()
    {
        return
        [
            (Entity.-- redacted --.Create(), null), // <ser:-- redacted -- />
            ( // -- redacted --
                Entity.-- redacted --.Default(() => Entity.-- redacted --.Create(-- redacted --)),
                SoapParserSettings.Soap12.WithOverride(new XPathWithValue("//pas:-- redacted --", "abc"))
            ),
            ( // -- redacted --
                Entity.-- redacted --.Default(Entity.-- redacted --.Default),
                SoapParserSettings.Soap12.WithOverride(new XPathWithValue("//pas:-- redacted --", "abc"))
            ),
            ( // -- redacted --
                Entity.-- redacted --.Default(() => Entity.-- redacted --.Default().With(p => p.-- redacted -- = "abc")), null
            ),
            ( // -- redacted --
              // -- redacted --
                Entity.-- redacted --.Create().With(r => r.-- redacted -- =
                [
                    Entity.-- redacted --.Create(-- redacted --),
                    Entity.-- redacted --.Create(-- redacted --),
                ]),
                SoapParserSettings.Soap12.WithOverride(new XPathWithValue("(//ser:-- redacted --[@-- redacted --='-- redacted --'])[1]", "a:something-else"))
            ),
            ( // order is incorrect, something like: -- redacted --
                Entity.-- redacted --.Default(() =>
                    Entity.-- redacted --.Default()
                        .With(p => p.-- redacted -- = [.. Entity.List(() => Entity.-- redacted --.Default(), total: 2)])
                        .With(p => p.-- redacted -- = Random.Shared.Next(minValue: 1, maxValue: 6))
                        .With(p => p.-- redacted -- = "test")
                        .With(p => p.-- redacted -- = "car"),
                    -- redacted --: 3),
                SoapParserSettings.Soap12
                    .WithElementToShuffle("//ser:-- redacted --[1]")
                    .WithElementToShuffle("//ser:-- redacted --[2]")
                    .WithElementToShuffle("//ser:-- redacted --[3]")
            ),
            ( // inside the <ser:-- redacted --> -> <AdditionalElement>AdditionalElementValue</AdditionalElement>
                Entity.-- redacted --.Default(Entity.-- redacted --.Default),
                SoapParserSettings.Soap12
                    .WithAdditionalElementToAdd(new XPathWithNameValuePairs("//ser:-- redacted --").Add("AdditionalElement", "AdditionalElementValue"))
            ),
            ( // <ser:-- redacted -- -- redacted --="..." -- redacted --="-- redacted --" AdditionalAttribute="AdditionalAttributeValue">
                Entity.-- redacted --.Default(Entity.-- redacted --.Default),
                SoapParserSettings.Soap12
                    .WithAdditionalAttributeToAdd(new XPathWithNameValuePairs("//ser:-- redacted --").Add("AdditionalAttribute", "AdditionalAttributeValue"))
            )
        ];
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    [DataRow(6)]
    [DataRow(7)]
    public async Task FailsSchemaValidationFor-- redacted --Request(int index)
    {
        // Arrange
        var (requestObject, settings) = -- redacted --()[index];

        // Act / Assert
        await this.AssertSchemaValidationErrorAsync(requestObject, settings, ServiceType);
    }

    private static List<(object, SoapParserSettings)> -- redacted --RequestData()
    {
        return
        [
            (Entity.-- redacted --.Create(), null),
        ];
    }

    [TestMethod]
    [DataRow(0)]
    public async Task FailsSchemaValidationFor-- redacted --Request(int index)
    {
        // Arrange
        var (requestObject, settings) = -- redacted --RequestData()[index];

        // Act / Assert
        await this.AssertSchemaValidationErrorAsync(requestObject, settings, ServiceType);
    }
}