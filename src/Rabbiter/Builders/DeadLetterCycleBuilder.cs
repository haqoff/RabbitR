using Rabbiter.Builders.Results;

namespace Rabbiter.Builders;

/// <summary>
/// Represents a builder of a mechanism for cyclic processing of unsuccessful messages through a dead letter exchange.
/// </summary>
public class DeadLetterCycleBuilder
{
    private TimeSpan _delay = TimeSpan.FromMinutes(5);
    private int _maxRetryCount = 10;
    private string? _customStartExchangeName;
    private string? _customEndExchangeName;
    private string? _customErrorQueueName;

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
    /// Sets the custom name of the exchange, which will receive messages processed with an error.
    /// If no custom name is specified, a default generated name like <c>{exchangeName}_{consumerGroup}_error_exchange_start</c> will be used.
    /// </summary>
    /// <param name="exchangeName">Custom error exchange name.</param>
    public DeadLetterCycleBuilder SetCustomStartExchangeName(string exchangeName)
    {
        _customStartExchangeName = exchangeName;
        return this;
    }

    /// <summary>
    /// Sets the custom name of the exchange, which will receive messages from the error queue after a delay (you can set custom value by calling <see cref="SetDelay"/>).
    /// This exchange will be connected to the main consumption queue. Thus, messages will be processed again from this exchange.
    /// If no custom name is specified, a default generated name like <c>{exchangeName}_{consumerGroup}_error_exchange_end</c> will be used.
    /// </summary>
    /// <param name="exchangeName">Custom exchange name.</param>
    public DeadLetterCycleBuilder SetCustomEndExchangeName(string exchangeName)
    {
        _customEndExchangeName = exchangeName;
        return this;
    }

    /// <summary>
    /// Sets the custom name of the queue that will be associated with the exchange, in which error messages will be added.
    /// After some delay, these messages will get to another exchange, which, in turn, will be connected to the main queue for consumption.
    /// If no custom name is specified, a default generated name like <c>{exchangeName}_{consumerGroup}_error_queue</c> will be used.
    /// </summary>
    /// <param name="queueName">Custom exchange name.</param>
    public DeadLetterCycleBuilder SetCustomQueueName(string queueName)
    {
        _customErrorQueueName = queueName;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal DeadLetterCycleBuildResult Build()
    {
        return new DeadLetterCycleBuildResult(_delay, _maxRetryCount, _customStartExchangeName, _customEndExchangeName, _customErrorQueueName);
    }
}