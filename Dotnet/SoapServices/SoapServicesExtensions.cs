using System;
using System.Linq;
using System.Text;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SoapCore;
using SoapCore.Extensibility;
using DefaultOperationInvoker = -- redacted --;

namespace -- redacted --;

/// <summary>
/// Helper class to register Soap Services in ASP.NET Core.
/// </summary>
public static class SoapServicesExtensions
{
    /// <summary>
    /// Register all the libraries to support SoapServices.
    /// </summary>
    /// <param name="services">
    /// Instance of <see cref="IServiceCollection"/> on which the services are registered.
    /// </param>
    /// <returns>
    /// Returns the instance of <paramref name="services"/> to support fluent method chaining.
    /// </returns>
    public static IServiceCollection RegisterSoapServices(this IServiceCollection services)
    {
        // This should be registered before SoapCore, otherwise the DefaultFaultExceptionTransformer will be used
        services.TryAddSingleton<IFaultExceptionTransformer, FaultExceptionTransformer>();
        services.TryAddSingleton<IOperationInvoker, AuthorizationOperationInvoker>();
        services.TryAddSingleton<IDefaultOperationInvoker, DefaultOperationInvoker>();

        _ = services.AddSoapCore();
        services.TryAddSingleton<IBindingFactory, BindingFactory>();
        return services;
    }

    /// <summary>
    /// Registers all the services for the SOAP endpoints in the service collection IOC container.
    /// </summary>
    /// <param name="services">
    /// Instance of <see cref="IServiceCollection"/> on which the services are registered.
    /// </param>
    /// <param name="providersToAddFunc">
    /// A func that returns an array of <see cref="SoapServicesProviderBase"/> to add.
    /// </param>
    public static void RegisterSoapServices(this IServiceCollection services,
        Func<SoapServicesProviderBase[]> providersToAddFunc)
    {
        _ = services.RegisterSoapServices();

        var soapServicesProviderFactory = new SoapServicesProviderFactory();
        _ = services.AddSingleton<ISoapServicesProviderFactory>(soapServicesProviderFactory);

        foreach (var provider in providersToAddFunc.Invoke())
        {
            soapServicesProviderFactory.AddProvider(provider);
            foreach (var (serviceDescriptor, messageInspector) in provider.ServicesRegistrationList)
            {
                services.TryAdd(serviceDescriptor);
                if (messageInspector != null)
                {
                    _ = services.AddSoapMessageInspector(messageInspector);
                }
            }
        }
    }

    /// <summary>
    /// Configure a middleware that serves the raw WSDL for a given match path. This middleware
    /// only executed on HTTP GET requests where the request path start with the 
    /// given <paramref name="prefixPath"/>.
    /// </summary>
    /// <param name="app">
    /// Instance of <see cref="IApplicationBuilder"/> pipeline, to hook the middleware onto.
    /// </param>
    /// <param name="prefixPath">
    /// The path at which the WSDL should be served.
    /// </param>
    /// <param name="descriptor">
    /// Instance of <see cref="SoapEndpointDescriptor"/> that describes an endpoint for a Soap service.
    /// </param>
    private static void SetupWsdlAndAdditionalFilesResponse(this IApplicationBuilder app,
        string prefixPath,
        SoapEndpointDescriptor descriptor)
    {
        var matchPath = Url.Combine(prefixPath, descriptor.ServiceMatchPath).ToLower();

        _ = app.Use(async (context, next) =>
          {
              var req = context.Request;
              var path = req.Path.Value?.ToLower();

              if (req.Method != "GET" || path == null)
              {
                  await next();
                  return;
              }

              // Serve additional files such as XSD files.
              foreach (var kv in descriptor.AdditionalFiles)
              {
                  var filePath = Url.Combine(prefixPath, kv.Key).ToLower();
                  if (path.Equals(filePath))
                  {
                      context.Response.Headers.Append("Content-Type", "text/xml");
                      await context.Response.WriteAsync(Encoding.UTF8.GetString(kv.Value));
                      return;
                  }
              }

              //  Service the WSDL files as a special kind.
              if (path.StartsWith(matchPath) && descriptor.RawWsdl != null)
              {
                  var fullPath = $"{req.Scheme}://{req.Host}{req.PathBase}{matchPath}";
                  var wsdl = Encoding.UTF8.GetString(descriptor.RawWsdl)
                      .Replace("[-- redacted --]", fullPath);

                  context.Response.Headers.Append("Content-Type", "text/xml");
                  await context.Response.WriteAsync(wsdl);
                  return;
              }

              await next();
          });
    }

    /// <summary>
    /// Register SOAP endpoints using the <see cref="SoapEndpointDescriptor"/> templates.
    /// </summary>
    /// <param name="app">
    /// Instance of <see cref="IApplicationBuilder"/> pipeline, to hook the endpoints onto.
    /// </param>
    /// <param name="servicesProvider">
    /// Instance <see cref="SoapServicesProviderBase"/> that provides the information which SoapServices
    /// needs to be registered.
    /// </param>
    /// <param name="prefixPath">
    /// Optional prefix path to move the location of the endpoints. If not null, 
    /// this string must start with a forward slash.
    /// </param>
    /// <param name="prependMiddleware">
    /// Callback to setup middlewares that must run before the setup of the SOAP 
    /// Service (e.g. authentication).
    /// </param>
    /// <param name="appendMiddleware">
    /// Callback to setup middlewares that must run after the setup of the SOAP
    /// Service (e.g. catch anything that is not handled).
    /// </param>
    private static void RegisterSoapEndpoints(this IApplicationBuilder app,
        SoapServicesProviderBase servicesProvider,
        string prefixPath,
        Action<IApplicationBuilder> prependMiddleware = null,
        Action<IApplicationBuilder> appendMiddleware = null)
    {
        var descriptors = servicesProvider.Descriptors();
        var endpoints = descriptors
            .Select(d => Url.Combine(prefixPath, d.ServiceMatchPath))
            .Select(p => p.ToLower())
            .ToList();

        endpoints.AddRange(descriptors
            .Select(d => d.AdditionalFiles)
            .SelectMany(a => a)
            .Select(k => Url.Combine(prefixPath, k.Key))
            .Select(u => u.ToLower()));

        _ = app.MapWhen(context => endpoints.Any(e => context.Request.Path.Value?.ToLower().StartsWith(e) ?? false), appBranch =>
          {
              foreach (var descriptor in descriptors)
              {
                  appBranch.SetupWsdlAndAdditionalFilesResponse(prefixPath, descriptor);
              }

              prependMiddleware?.Invoke(appBranch);

              foreach (var descriptor in descriptors)
              {
                  var resolvedPath = Url.Combine(prefixPath, descriptor.ServiceMatchPath).ToLower();
                  var options = new SoapEncoderOptions
                  {
                      // The encodeShouldEmitUTFIdentifier to false is needed to avoid including ï»¿ (BOM), which give an issue with SOAPUI.
                      WriteEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                      MessageVersion = descriptor.MessageVersion
                  };

                  _ = appBranch.UseRouting();
                  _ = appBranch.UseEndpoints(e =>
                  {
                      _ = e.UseSoapEndpoint(descriptor.ServiceType, resolvedPath, options, descriptor.SoapSerializer,
                          caseInsensitivePath: true, omitXmlDeclaration: false);
                  });
              }

              appendMiddleware?.Invoke(appBranch);
          });

        -- redacted --
    }

    /// <summary>
    /// Configure all Soap endpoints.
    /// </summary>
    /// <param name="app">
    /// Instance of <see cref="IApplicationBuilder"/> pipeline, to hook the endpoints onto.
    /// </param>
    /// <param name="afterHttpsRedirection">Is the setup happening after HttpsRedirection or not.</param>
    /// <param name="prependMiddleware">
    /// Callback to setup middlewares that must run before the setup of the SOAP 
    /// Service (e.g. authentication).
    /// </param>
    /// <param name="appendMiddleware">
    /// Callback to setup middlewares that must run after the setup of the SOAP
    /// Service (e.g. catch anything that is not handled).
    /// </param>
    public static void SetupSoapEndpoints(this IApplicationBuilder app,
        bool afterHttpsRedirection,
        Action<IApplicationBuilder> prependMiddleware = null,
        Action<IApplicationBuilder> appendMiddleware = null)
    {
        var providerFactory = app.ApplicationServices.GetRequiredService<ISoapServicesProviderFactory>();
        var optionsMonitor = app.ApplicationServices.GetRequiredService<IOptionsMonitor<-- redacted -->>();
        var soapServicesProvider = providerFactory.GetSoapServicesProvider(optionsMonitor, afterHttpsRedirection);

        if (soapServicesProvider == null)
        {
            return;
        }

        var routePrefix = soapServicesProvider.GetRoutePrefix(optionsMonitor);
        app.RegisterSoapEndpoints(soapServicesProvider, routePrefix, prependMiddleware, appendMiddleware);
    }
}
