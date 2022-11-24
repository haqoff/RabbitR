using RabbitR.Builders.Instances;

namespace RabbitR.Consumers;

/// <summary>
/// Provides a method that starts message consumption and processing.
/// </summary>
internal interface IConsumer
{
    /// <summary>
    /// Subscribes to the specified messages and starts processing.
    /// </summary>
    /// <param name="instance">The instance for which to start consuming messages.</param>
    /// <param name="cancellationToken">Processing cancellation token.</param>
    void Start(InstanceBuildResult instance, CancellationToken cancellationToken);
}