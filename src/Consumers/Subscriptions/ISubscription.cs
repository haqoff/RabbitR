using RabbitR.Consumers.Configurations;

namespace RabbitR.Consumers.Subscriptions;

/// <summary>
/// Contains subscription information.
/// </summary>
internal interface ISubscription
{
    /// <summary>
    /// The name of the object.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The maximum number of messages that can be processed in parallel within this subscription.
    /// Must be less than or equal to <see cref="ConsumerConfig.MaxDegreeOfParallelism"/>.
    /// </summary>
    public ushort MaxDegreeOfParallelism { get; }

    /// <summary>
    /// The type of message into which the received bytes will be deserialized.
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// Handler type.
    /// </summary>
    public Type HandlerType { get; }
}