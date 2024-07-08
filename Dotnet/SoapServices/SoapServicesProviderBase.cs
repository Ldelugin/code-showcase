using System;
using System.Collections.Generic;
using -- redacted --;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoapCore.Extensibility;

namespace -- redacted --;

/// <summary>
/// Provider base class to describe what SoapServices should be registered.
/// </summary>
public abstract class SoapServicesProviderBase
{
    /// <summary>
    /// Instantiates a new instance of <see cref="SoapServicesProviderBase"/>.
    /// </summary>
    protected SoapServicesProviderBase()
    {
        this.ServicesRegistrationList = [];
    }

    /// <summary>
    /// List that contains the descriptions what soap services needs to be registered.
    /// </summary>
    public List<(ServiceDescriptor ServiceDescriptor, IMessageInspector2 MessageInspector)> ServicesRegistrationList { get; }

    /// <summary>
    /// Array with all the soap endpoint descriptors.
    /// </summary>
    /// <returns>Returns a new array of <see cref="SoapEndpointDescriptor"/>.</returns>
    public abstract SoapEndpointDescriptor[] Descriptors();

    /// <summary>
    /// Predicate whether the registered soap services needs to be setup or not.
    /// </summary>
    /// <param name="optionsMonitor">
    /// An instance of <see cref="IOptionsMonitor{TOptions}"/> for getting the configuration.
    /// </param>
    /// <param name="afterHttpsRedirection">Is the setup happening after HttpsRedirection or not.</param>
    /// <returns>Returns true if the soap services needs to be setup; otherwise false.</returns>
    public abstract bool SetupPredicate(IOptionsMonitor<-- redacted --> optionsMonitor, bool afterHttpsRedirection);

    /// <summary>
    /// Get the route prefix from the given <paramref name="optionsMonitor"/>.
    /// </summary>
    /// <param name="optionsMonitor">
    /// An instance of <see cref="IOptionsMonitor{TOptions}"/> for getting the configuration.
    /// </param>
    /// <returns>Returns the route prefix.</returns>
    public abstract string GetRoutePrefix(IOptionsMonitor<-- redacted --> optionsMonitor);

    -- redacted --

    /// <summary>
    /// Are the SoapServices allowed to use HTTP.
    /// </summary>
    protected abstract bool AllowedToUseHttp { get; }
}
