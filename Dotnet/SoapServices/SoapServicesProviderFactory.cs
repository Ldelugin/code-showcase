using System;
using System.Collections.Generic;
using System.Linq;
using -- redacted --;
using Microsoft.Extensions.Options;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="ISoapServicesProviderFactory"/>.
/// </summary>
internal class SoapServicesProviderFactory : ISoapServicesProviderFactory
{
    private readonly IList<SoapServicesProviderBase> soapServicesProviders;

    /// <summary>
    /// Instantiates a new instance of <see cref="SoapServicesProviderFactory"/>.
    /// </summary>
    public SoapServicesProviderFactory()
    {
        this.soapServicesProviders = [];
    }

    /// <summary>
    /// Add the given <see cref="SoapServicesProviderBase"/>.
    /// </summary>
    /// <param name="soapServicesProviderBase">
    /// The instance of <see cref="SoapServicesProviderBase"/> to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="soapServicesProviderBase"/> is null.
    /// </exception>
    public void AddProvider(SoapServicesProviderBase soapServicesProviderBase)
    {
        if (soapServicesProviderBase == null)
        {
            throw new ArgumentNullException(nameof(soapServicesProviderBase));
        }

        this.soapServicesProviders.Add(soapServicesProviderBase);
    }

    /// <summary>
    /// Retrieve a <see cref="SoapServicesProviderBase"/> depending on the given <paramref name="optionsMonitor"/>.
    /// </summary>
    /// <param name="optionsMonitor">
    /// An instance of <see cref="IOptionsMonitor{TOptions}"/> for getting the configuration.
    /// </param>
    /// <param name="afterHttpsRedirection">Is the setup happening after HttpsRedirection or not.</param>
    /// <returns>
    /// Returns an instance of <see cref="SoapServicesProviderBase"/> that.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="optionsMonitor"/> is null.
    /// </exception>
    public SoapServicesProviderBase GetSoapServicesProvider(IOptionsMonitor<-- redacted --> optionsMonitor,
        bool afterHttpsRedirection)
    {
        if (optionsMonitor == null)
        {
            throw new ArgumentNullException(nameof(optionsMonitor));
        }

        return this.soapServicesProviders.FirstOrDefault(p =>
            p.SetupPredicate(optionsMonitor, afterHttpsRedirection));
    }
}
