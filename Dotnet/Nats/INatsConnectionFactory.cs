using NATS.Client;

namespace -- redacted --;

/// <summary>
/// Defines the factory to create an <see cref="IConnection"/>.
/// </summary>
public interface INatsConnectionFactory
{
    /// <summary>
    /// Create a new <see cref="IConnection"/>.
    /// </summary>
    /// <param name="options">Instance of a <see cref="INatsConnectionOptions"/>.</param>
    /// <returns>A new instance of <see cref="IConnection"/>.</returns>
    IConnection CreateConnection(INatsConnectionOptions options);
}