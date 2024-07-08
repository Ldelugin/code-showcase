using System;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Extension methods to add NATS.
/// </summary>
public static class NatsServiceCollectionExtensions
{
    /// <summary>
    /// The default function to retrieve the section named '-- redacted --:NatsOptions'.
    /// </summary>
    private static Func<IConfiguration, IConfiguration> DefaultGetSectionFunction => root =>
        root.GetSection(ConfigurationPath.Combine(-- redacted --, NatsOptions.Name));

    /// <summary>
    /// The default function to retrieve the section named '-- redacted --:NatsOptions:NatsServerOptions'.
    /// </summary>
    private static Func<IConfiguration, IConfiguration> DefaultGetNatsServerOptionsFunction => root =>
        root.GetSection(ConfigurationPath.Combine(-- redacted --.Name, NatsOptions.Name,
            NatsServerOptions.Name));

    /// <summary>
    /// Add NATS and all it's required dependencies to the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">
    /// The instance of <see cref="IServiceCollection"/> to add the dependencies to.
    /// </param>
    /// <param name="configuration">
    /// The instance of a specific <see cref="IConfiguration"/> to add the configuration change token source
    /// and named configure from.
    /// </param>
    /// <param name="configure">An optional configure action.</param>
    /// <returns>
    /// Returns the same instance as provided with <paramref name="serviceCollection"/> to enable fluent method chaining.
    /// </returns>
    public static IServiceCollection AddNats(this IServiceCollection serviceCollection, IConfiguration configuration,
        Action<Options> configure = null)
    {
        return serviceCollection
            .AddNatsInternal(configure)
            // This change token is used in NatsConnection.cs
            .AddOptionsChangeTokenSource<NatsOptions>(configuration, DefaultGetSectionFunction)
            .AddNamedConfigureFromConfigurationOptions<NatsOptions>(configuration, DefaultGetSectionFunction)
            .AddNamedConfigureFromConfigurationOptions<NatsServerOptions>(configuration, DefaultGetNatsServerOptionsFunction);
    }

    /// <summary>
    /// Add NATS and all it's required dependencies to the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">
    /// The instance of <see cref="IServiceCollection"/> to add the dependencies to.
    /// </param>
    /// <param name="configure">An optional configure action.</param>
    /// <returns>
    /// Returns the same instance as provided with <paramref name="serviceCollection"/> to enable fluent method chaining.
    /// </returns>
    private static IServiceCollection AddNatsInternal(this IServiceCollection serviceCollection,
        Action<Options> configure = null)
    {
        var options = ConnectionFactory.GetDefaultOptions();
        configure?.Invoke(options);

        serviceCollection.TryAddSingleton<ConnectionFactory>();
        serviceCollection.TryAddSingleton(options);
        serviceCollection.TryAddSingleton<INatsConnectionFactory, NatsConnectionFactory>();
        serviceCollection.TryAddSingleton<INatsConnectionOptions, NatsConnectionOptions>();
        serviceCollection.TryAddSingleton<INatsConnection, NatsConnection>();
        serviceCollection.TryAddSingleton<ITimerFactory, TimerFactory>();
        serviceCollection.TryAddSingleton<INatsServerOptionsManager, NatsServerOptionsManager>();

        return serviceCollection;
    }

    /// <summary>
    /// Add the <see cref="NatsMessagesPoster"/> as implementation of <see cref="IMessagePoster"/>
    /// to the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">
    /// The instance of <see cref="IServiceCollection"/> to add the dependencies to.
    /// </param>
    /// <returns>
    /// Returns the same instance as provided with <paramref name="serviceCollection"/> to enable fluent method chaining.
    /// </returns>
    public static IServiceCollection AddNatsMessagePoster(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IMessagePoster, NatsMessagesPoster>();
        return serviceCollection;
    }

    /// <summary>
    /// Add the <see cref="NatsMessagesReceiver"/> as implementation of <see cref="IMessageReceiver"/>
    /// to the <paramref name="serviceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">
    /// The instance of <see cref="IServiceCollection"/> to add the dependencies to.
    /// </param>
    /// <returns>
    /// Returns the same instance as provided with <paramref name="serviceCollection"/> to enable fluent method chaining.
    /// </returns>
    public static IServiceCollection AddNatsMessageReceiver(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IMessageReceiver>(serviceProvider =>
        {
            var natsConfiguration = serviceProvider.GetRequiredService<INatsConnection>();
            var logger = serviceProvider.GetRequiredService<ILogger<NatsMessagesReceiver>>();
            var messageReceiver = new NatsMessagesReceiver(natsConfiguration, logger);
            return messageReceiver;
        });
        return serviceCollection;
    }
}