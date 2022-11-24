using RabbitR.Consumers.Configurations;
using RabbitR.Consumers.Subscriptions;
using RabbitR.Messages;

namespace RabbitR.Builders.Instances.Consumers;

/// <summary>
/// Represents a subscription builder for an queue.
/// </summary>
/// <typeparam name="TMessage">Message type.</typeparam>
public class QueueSubscriptionBuilder<TMessage> where TMessage : IEventBusMessage
{
    private readonly string _name;
    private readonly Type _handlerType;
    private ushort _maxDegreeOfParallelism = 1;

    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueSubscriptionBuilder{TMessage}"/>.
    /// </summary>
    internal QueueSubscriptionBuilder(string name, Type handlerType)
    {
        _name = name;
        _handlerType = handlerType;
    }

    /// <summary>
    /// Specifies how many threads can simultaneously process messages from this queue.
    /// The value must be less than or equal to <see cref="ConsumerConfig.MaxDegreeOfParallelism"/>.
    /// The default is 1.
    /// </summary>
    public QueueSubscriptionBuilder<TMessage> SetMaxDegreeOfParallelism(ushort degree)
    {
        _maxDegreeOfParallelism = degree;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal QueueSubscription Build()
    {
        return new QueueSubscription(_name, typeof(TMessage), _handlerType, _maxDegreeOfParallelism);
    }
}