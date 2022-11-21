using Rabbiter.Builders.Results;
using Rabbiter.Consumers.Configurations;
using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Messages;

namespace Rabbiter.Builders;

/// <summary>
/// Represents a subscription builder for an exchange.
/// </summary>
/// <typeparam name="TMessage">Message type.</typeparam>
public class ExchangeSubscriptionBuilder<TMessage> where TMessage : IEventBusMessage
{
    private readonly string _name;
    private ushort _maxDegreeOfParallelism = 1;
    private DeadLetterCycleBuildResult? _deadLetterCycle;
    private string? _customQueueName;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeSubscriptionBuilder{TMessage}"/>.
    /// </summary>
    internal ExchangeSubscriptionBuilder(string name)
    {
        _name = name;
    }

    /// <summary>
    /// Specifies that in case of an error during message processing, this message should be sent to a dead letter exchanger.
    /// After a while, this message will again fall from the dead letter exchanger to this exchanger.
    /// Thus, if processing fails, there will be several attempts to process the message successfully.
    /// Disabled by default.
    /// </summary>
    /// <param name="action">An action that configures the handling of the dead letter exchanger.</param>
    public ExchangeSubscriptionBuilder<TMessage> UseDeadLetter(Action<DeadLetterCycleBuilder> action)
    {
        var builder = new DeadLetterCycleBuilder();
        action(builder);
        _deadLetterCycle = builder.Build();

        return this;
    }

    /// <summary>
    /// Specifies how many threads can simultaneously process messages from this exchange.
    /// The value must be less than or equal to <see cref="ConsumerConfig.MaxDegreeOfParallelism"/>.
    /// The default is 1.
    /// </summary>
    public ExchangeSubscriptionBuilder<TMessage> SetMaxDegreeOfParallelism(ushort degree)
    {
        _maxDegreeOfParallelism = degree;
        return this;
    }

    /// <summary>
    /// Sets the custom name of the queue that will be associated with the exchange and from which consumption will occur.
    /// If no custom name is specified, a default generated name like <c>{exchangeName}_{consumerGroup}_main_queue</c> will be used.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    public ExchangeSubscriptionBuilder<TMessage> SetCustomAssociatedQueueName(string queueName)
    {
        _customQueueName = queueName;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal ExchangeSubscription Build()
    {
        return new ExchangeSubscription(_name, typeof(TMessage), _maxDegreeOfParallelism, _customQueueName, _deadLetterCycle);
    }
}