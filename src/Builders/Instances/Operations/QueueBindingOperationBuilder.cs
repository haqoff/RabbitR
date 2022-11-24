using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Represents the builder of the queue binding operation.
/// </summary>
public class QueueBindingOperationBuilder : IQueueBindingOperationNoNameBuilder
{
    private string? _queue;
    private string? _exchange;
    private string _routingKey = "";
    private IDictionary<string, object>? _arguments;

    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueBindingOperationBuilder"/>.
    /// </summary>
    internal QueueBindingOperationBuilder()
    {
    }

    /// <summary>
    /// Sets the name of the queue to be associated.
    /// </summary>
    public QueueBindingOperationBuilder SetQueueName(string queueName)
    {
        _queue = queueName;
        return this;
    }

    /// <summary>
    /// Sets the name of the exchange to associate the queue with.
    /// </summary>
    public QueueBindingOperationBuilder SetExchangeName(string exchangeName)
    {
        _exchange = exchangeName;
        return this;
    }

    /// <summary>
    /// Sets the routing key, only those messages that are published in exchange with this key will get into the associated queue.
    /// The default value is an empty string.
    /// </summary>
    public QueueBindingOperationBuilder SetRoutingKey(string routingKey)
    {
        _routingKey = routingKey;
        return this;
    }

    /// <summary>
    /// Sets the binding arguments.
    /// </summary>
    public QueueBindingOperationBuilder SetArguments(IDictionary<string, object> arguments)
    {
        _arguments = arguments;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal QueueBindingOperation Build()
    {
        return new QueueBindingOperation(_queue, _exchange, _routingKey, _arguments);
    }

    IQueueBindingOperationNoNameBuilder IQueueBindingOperationNoNameBuilder.SetRoutingKey(string routingKey) => SetRoutingKey(routingKey);
    IQueueBindingOperationNoNameBuilder IQueueBindingOperationNoNameBuilder.SetArguments(IDictionary<string, object> arguments) => SetArguments(arguments);
}