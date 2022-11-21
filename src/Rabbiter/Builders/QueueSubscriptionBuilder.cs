using Rabbiter.Consumers.Configurations;
using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Messages;

namespace Rabbiter.Builders;

/// <summary>
/// Represents a subscription builder for an queue.
/// </summary>
/// <typeparam name="TMessage">Message type.</typeparam>
public class QueueSubscriptionBuilder<TMessage> where TMessage : IEventBusMessage
{
    private readonly string _name;
    private ushort _maxDegreeOfParallelism = 1;

    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueSubscriptionBuilder{TMessage}"/>.
    /// </summary>
    internal QueueSubscriptionBuilder(string name)
    {
        _name = name;
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
        return new QueueSubscription(_name, typeof(TMessage), _maxDegreeOfParallelism);
    }
}