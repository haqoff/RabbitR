using Rabbiter.Builders.Results;

namespace Rabbiter.Consumers.Subscriptions;

/// <summary>
/// Represents a subscription to an exchanger.
/// </summary>
internal class ExchangeSubscription : ISubscription
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeSubscription"/>.
    /// </summary>
    internal ExchangeSubscription(string name, Type messageType, ushort maxDegreeOfParallelism, string? customQueueName, DeadLetterCycleBuildResult? deadLetterCycle)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The name must be specified.", nameof(name));
        }

        if (maxDegreeOfParallelism < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "The max degree of parallelism cannot be less than 1.");
        }

        if (customQueueName is not null && string.IsNullOrWhiteSpace(customQueueName))
        {
            throw new ArgumentException("Custom queue name cannot be empty string.", nameof(customQueueName));
        }

        Name = name;
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        CustomQueueName = customQueueName;
        DeadLetterCycle = deadLetterCycle;
        MessageType = messageType;
    }

    /// <summary>
    /// The name of the exchange.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Dead letter cyclic mechanism for cyclic processing of unsuccessful messages through a dead letter exchange.
    /// </summary>
    public DeadLetterCycleBuildResult? DeadLetterCycle { get; }

    /// <summary>
    /// The maximum number of threads that can process messages from this exchange in parallel.
    /// </summary>
    public ushort MaxDegreeOfParallelism { get; }

    /// <summary>
    /// The name of the queue that will be associated with the exchange named <see cref="Name"/> and from which consumption will occur.
    /// If not specified, the default name will be used.
    /// </summary>
    public string? CustomQueueName { get; }

    /// <summary>
    /// The type of message into which the received bytes will be deserialized.
    /// </summary>
    public Type MessageType { get; }
}