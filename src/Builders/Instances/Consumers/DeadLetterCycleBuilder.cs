using RabbitR.Builders.Instances.Operations;
using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Consumers;

/// <summary>
/// Represents a builder of a mechanism for cyclic processing of unsuccessful messages through a dead letter exchange.
/// </summary>
public class DeadLetterCycleBuilder
{
    private TimeSpan _delay = TimeSpan.FromMinutes(5);
    private int _maxRetryCount = 10;
    private ExchangeDeclarationOperation? _customStartExchange;
    private ExchangeDeclarationOperation? _customEndExchange;
    private QueueDeclarationOperation? _customQueue;
    private QueueBindingOperation? _customStartQueueBinding;
    private QueueBindingOperation? _customMainQueueBinding;

    /// <summary>
    /// Initializes a new instance of the class <see cref="DeadLetterCycleBuilder"/>.
    /// </summary>
    internal DeadLetterCycleBuilder()
    {
    }

    /// <summary>
    /// Sets the delay after which an unsuccessful message will be published again.
    /// Default value is 5 minutes.
    /// </summary>
    public DeadLetterCycleBuilder SetDelay(TimeSpan delay)
    {
        _delay = delay;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of attempts to reprocess the message.
    /// Default value is 10.
    /// </summary>
    public DeadLetterCycleBuilder SetMaxRetryCount(int maxRetryCount)
    {
        _maxRetryCount = maxRetryCount;
        return this;
    }

    /// <summary>
    /// Configures the creation of an exchange that will receive messages after unsuccessful processing.
    /// For example, you can set your own exchange name. The default name is <c>{exchangeName}_{consumerGroup}_error_exchange_start</c>.
    /// </summary>
    public DeadLetterCycleBuilder ConfigureStartExchange(Action<ExchangeDeclarationOperationBuilder> action)
    {
        var builder = new ExchangeDeclarationOperationBuilder(null);
        action(builder);
        _customStartExchange = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the creation of an exchange that will receive messages from the queue after a timeout.
    /// This exchange will be associated with the main queue from which messages are consumed.
    /// For example, you can set your own exchange name.
    /// The default name is <c>{exchangeName}_{consumerGroup}_error_exchange_end</c>.
    /// </summary>
    public DeadLetterCycleBuilder ConfigureEndExchange(Action<ExchangeDeclarationOperationBuilder> action)
    {
        var builder = new ExchangeDeclarationOperationBuilder(null);
        action(builder);
        _customEndExchange = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the creation of a queue that will be associated with the Start exchange and, after a timeout, will republish to the End exchanger.
    /// For example, you can set your own queue name. The default name is <c>{exchangeName}_{consumerGroup}_error_queue</c>.
    /// Also, arguments are automatically set for this queue: x-message-ttl (equal to timeout) and x-dead-letter-exchange, which is equal to the name of the exchange End.
    /// If you specify custom arguments, they will be merged with the standard ones.
    /// </summary>
    public DeadLetterCycleBuilder ConfigureQueue(Action<QueueDeclarationOperationBuilder> action)
    {
        var builder = new QueueDeclarationOperationBuilder(null);
        action(builder);
        _customQueue = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the binding between the Start exchange, which will receive unsuccessful messages, and the error queue, in which messages will wait for the timeout.
    /// </summary>
    public DeadLetterCycleBuilder ConfigureStartBinding(Action<IQueueBindingOperationNoNameBuilder> action)
    {
        var builder = new QueueBindingOperationBuilder();
        action(builder);
        _customStartQueueBinding = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures the binding between the End exchange and the main queue on which to consume.
    /// </summary>
    public DeadLetterCycleBuilder ConfigureMainBinding(Action<IQueueBindingOperationNoNameBuilder> action)
    {
        var builder = new QueueBindingOperationBuilder();
        action(builder);
        _customMainQueueBinding = builder.Build();
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal DeadLetterCycleBuildResult Build()
    {
        return new DeadLetterCycleBuildResult(
            _delay,
            _maxRetryCount,
            _customStartExchange,
            _customEndExchange,
            _customQueue,
            _customStartQueueBinding,
            _customMainQueueBinding
        );
    }
}