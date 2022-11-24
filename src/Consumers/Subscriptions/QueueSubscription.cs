using RabbitR.Consumers.Configurations;

namespace RabbitR.Consumers.Subscriptions;

/// <summary>
/// Represents a subscription to queue.
/// </summary>
internal class QueueSubscription : ISubscription
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueSubscription"/>.
    /// </summary>
    internal QueueSubscription(string name, Type messageType, Type handlerType, ushort maxDegreeOfParallelism)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name must be specified.", nameof(name));
        }

        if (maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "The max degree of parallelism cannot be less than 1.");
        }

        Name = name;
        MessageType = messageType;
        HandlerType = handlerType;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
    }

    /// <summary>
    /// The name of the queue.
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