using System;
using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Implementation of <see cref="INatsConnectionFactory"/> which acts as a wrapper
/// around <see cref="ConnectionFactory"/> to support unit testing.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="NatsConnectionFactory"/>.
/// </remarks>
/// <param name="connectionFactory">Instance of <see cref="ConnectionFactory"/>.</param>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionFactory"/> is null.</exception>
public class NatsConnectionFactory(ConnectionFactory connectionFactory) : INatsConnectionFactory
{
    private readonly ConnectionFactory connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    /// <summary>
    /// Create a new <see cref="IConnection"/>.
    /// </summary>
    /// <param name="options">Instance of a <see cref="INatsConnectionOptions"/>.</param>
    /// <returns>A new instance of <see cref="IConnection"/>.</returns>
    public IConnection CreateConnection(INatsConnectionOptions options)
        => this.connectionFactory.CreateConnection(options.InternalNatsOptions);
}