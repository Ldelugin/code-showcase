using System;
using -- redacted --;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Interface to register and retrieve <see cref="SoapServicesProviderBase"/>'s from. 
/// </summary>
public interface ISoapServicesProviderFactory
{
    /// <summary>
    /// Add the given <see cref="SoapServicesProviderBase"/>.
    /// </summary>
    /// <param name="soapServicesProviderBase">
    /// The instance of <see cref="SoapServicesProviderBase"/> to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="soapServicesProviderBase"/> is null.
    /// </exception>
    void AddProvider(SoapServicesProviderBase soapServicesProviderBase);

    /// <summary>
    /// Retrieve a <see cref="SoapServicesProviderBase"/> depending on the given <paramref name="configuration"/>.
    /// </summary>
    /// <param name="optionsMonitor"></param>
    /// <param name="afterHttpsRedirection">Is the setup happening after HttpsRedirection or not.</param>
    /// <returns>
    /// Returns an instance of <see cref="SoapServicesProviderBase"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="optionsMonitor"/> is null.
    /// </exception>
    SoapServicesProviderBase GetSoapServicesProvider(IOptionsMonitor<-- redacted --> optionsMonitor,
        bool afterHttpsRedirection);
}
