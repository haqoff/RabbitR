using RabbitR.Builders.Instances.Operations;
using RabbitR.Builders.Instances.Operations.Results;
using RabbitR.Consumers.Configurations;
using RabbitR.Consumers.Subscriptions;
using RabbitR.Messages;

namespace RabbitR.Builders.Instances.Consumers;

/// <summary>
/// Represents a subscription builder for an exchange.
/// </summary>
/// <typeparam name="TMessage">Message type.</typeparam>
public class ExchangeSubscriptionBuilder<TMessage> where TMessage : IEventBusMessage
{
    private readonly string _name;
    private readonly Type _handlerType;
    private ushort _maxDegreeOfParallelism = 1;
    private DeadLetterCycleBuildResult? _deadLetterCycle;
    private ExchangeDeclarationOperation? _customExchangeCreation;
    private QueueDeclarationOperation? _customQueueCreation;
    private QueueBindingOperation? _customQueueBinding;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeSubscriptionBuilder{TMessage}"/>.
    /// </summary>
    internal ExchangeSubscriptionBuilder(string name, Type handlerType)
    {
        _name = name;
        _handlerType = handlerType;
    }

    /// <summary>
    /// Specifies that in case of an error during message processing, this message should be sent to a dead letter exchange.
    /// After a while, this message will again fall from the dead letter exchange to main queue.
    /// Thus, if processing fails, there will be several attempts to process the message successfully.
    /// Disabled by default.
    /// </summary>
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
    /// Configures the creation of an exchange.
    /// For example, you can set your own type or creation arguments.
    /// </summary>
    public ExchangeSubscriptionBuilder<TMessage> ConfigureExchange(Action<IExchangeDeclarationOperationNoNameBuilder> builderAction)
    {
        var builder = new ExchangeDeclarationOperationBuilder(null);
        builderAction(builder);
        _customExchangeCreation = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the creation of a queue to be associated with this exchange.
    /// For example, you can set your own name. The default name is <c>{exchangeName}_{consumerGroup}_main_queue</c>.
    /// The x-dead-letter-exchange parameter may be added to the creation arguments if the dead letter mechanism is configured.
    /// If you specify custom arguments, they will be merged with the standard ones.
    /// </summary>
    public ExchangeSubscriptionBuilder<TMessage> ConfigureQueue(Action<QueueDeclarationOperationBuilder> builderAction)
    {
        var builder = new QueueDeclarationOperationBuilder(null);
        builderAction(builder);
        _customQueueCreation = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the binding between the queue and the exchange.
    /// For example, you can specify a routing key to receive only certain messages.
    /// </summary>
    public ExchangeSubscriptionBuilder<TMessage> ConfigureBinding(Action<IQueueBindingOperationNoNameBuilder> builderAction)
    {
        var builder = new QueueBindingOperationBuilder();
        builderAction(builder);
        _customQueueBinding = builder.Build();
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal ExchangeSubscription Build()
    {
        return new ExchangeSubscription(
            _name,
            typeof(TMessage),
            _handlerType,
            _maxDegreeOfParallelism,
            _deadLetterCycle,
            _customExchangeCreation,
            _customQueueCreation,
            _customQueueBinding
        );
    }
}