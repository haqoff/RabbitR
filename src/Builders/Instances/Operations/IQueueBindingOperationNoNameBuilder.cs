namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Provides methods for building a queue binding operation without specifying names.
/// </summary>
public interface IQueueBindingOperationNoNameBuilder
{
    /// <summary>
    /// Sets the routing key, only those messages that are published in exchange with this key will get into the associated queue.
    /// The default value is an empty string.
    /// </summary>
    IQueueBindingOperationNoNameBuilder SetRoutingKey(string routingKey);

    /// <summary>
    /// Sets the binding arguments.
    /// </summary>
    IQueueBindingOperationNoNameBuilder SetArguments(IDictionary<string, object> arguments);
}