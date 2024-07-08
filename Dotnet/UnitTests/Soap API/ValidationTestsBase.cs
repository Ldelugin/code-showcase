using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using -- redacted --;
using TestsSupport.Soap;
using TestsSupport.Soap.Extensions;

namespace -- redacted --;

public abstract class ValidationTestsBase
{
    protected static -- redacted --<Startup> WebApplicationFactory { get; set; }
    protected const string UserName = "-- redacted --";
    protected const string Password = "-- redacted --";
    private HttpRequestMessage httpRequestMessage;

    protected abstract SoapVersion SoapVersion { get; }
    protected abstract string RelativeRequestUri { get; }

    public TestContext TestContext { get; set; }

    public virtual void Cleanup() => this.httpRequestMessage?.Dispose();

    protected async Task AssertStructureValidationErrorAsync(string soapRequest, SoapVersion? soapVersion = null)
    {
        // Arrange / Act
        soapVersion ??= this.SoapVersion;
        var soapFaultMessage = await this.PerformSoapCallAsync(soapRequest);

        // Assert
        _ = soapFaultMessage.IsNotNull()
            .HasExpectedCode(soapVersion.Value)
            .HasInternalServerErrorAsReason()
            .HasReason(r => r.Contains("Internal server error"));
    }

    protected async Task AssertSchemaValidationErrorAsync(object requestObject, SoapParserSettings settings, Type serviceType)
    {
        // Arrange
        this.TestContext.WriteLine($"Request object: {System.Text.Json.JsonSerializer.Serialize(requestObject)}");
        var soapRequest = requestObject.AsSoap(settings ?? new SoapParserSettings().WithSoapVersion(this.SoapVersion));
        this.TestContext.WriteLine($"Soap request: {soapRequest}");
        var expectedValidationErrors = await SoapValidator.GetExpectedValidationErrorsAsync(soapRequest, serviceType);

        // Act
        var soapFaultMessage = await this.PerformSoapCallAsync(soapRequest);

        // Assert
        _ = soapFaultMessage.IsNotNull()
            .HasSenderAsCode()
            .HasBasRequestAsReason()
            .HasDefaultInvalidDetailMessage();

        foreach (var expectedValidationError in expectedValidationErrors)
        {
            _ = soapFaultMessage.HasReason(expectedValidationError.Message);
        }
    }

    private async Task<SoapFaultMessage> PerformSoapCallAsync(string soapRequest)
    {
        var client = WebApplicationFactory.CreateClient();
        this.httpRequestMessage = SoapRequestHelpers.CreateSoapRequest(
            soapRequest, this.RelativeRequestUri, UserName, Password);

        var responseMessage = await client.SendAsync(this.httpRequestMessage);
        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        this.TestContext.WriteLine($"Response: {responseContent}");
        return SoapFaultMessage.Parse(responseContent, this.SoapVersion);
    }
}