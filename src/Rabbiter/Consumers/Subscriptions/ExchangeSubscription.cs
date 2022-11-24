using Rabbiter.Builders.Instances.Consumers;
using Rabbiter.Builders.Instances.Operations.Results;

namespace Rabbiter.Consumers.Subscriptions;

/// <summary>
/// Represents a subscription to an exchanger.
/// </summary>
internal class ExchangeSubscription : ISubscription
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeSubscription"/>.
    /// </summary>
    internal ExchangeSubscription(
        string name,
        Type messageType,
        Type handlerType,
        ushort maxDegreeOfParallelism,
        DeadLetterCycleBuildResult? deadLetterCycle,
        ExchangeDeclarationOperation? customExchange,
        QueueDeclarationOperation? customQueue, QueueBindingOperation? customBinding)
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
        MaxDegreeOfParallelism = maxDegreeOfParallelism;
        DeadLetterCycle = deadLetterCycle;
        CustomExchange = customExchange;
        CustomQueue = customQueue;
        CustomBinding = customBinding;
        MessageType = messageType;
        HandlerType = handlerType;
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
    /// The type of message into which the received bytes will be deserialized.
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// Handler type.
    /// </summary>
    public Type HandlerType { get; }

    /// <summary>
    /// Customization of the creation of the exchange.
    /// </summary>
    public ExchangeDeclarationOperation? CustomExchange { get; }

    /// <summary>
    /// Customization of the creation of the queue.
    /// </summary>
    public QueueDeclarationOperation? CustomQueue { get; }

    /// <summary>
    /// Custom binding operation.
    /// </summary>
    public QueueBindingOperation? CustomBinding { get; }
}