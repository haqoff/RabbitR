using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Represents the builder of the exchange binding operation.
/// </summary>
public class ExchangeBindingOperationBuilder
{
    private string? _source;
    private string? _destination;
    private string _routingKey = "";
    private IDictionary<string, object>? _arguments;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeBindingOperationBuilder"/>.
    /// </summary>
    internal ExchangeBindingOperationBuilder()
    {
    }

    /// <summary>
    /// Sets the name of the exchange where the messages will go..
    /// </summary>
    public ExchangeBindingOperationBuilder SetDestination(string destination)
    {
        _destination = destination;
        return this;
    }

    /// <summary>
    /// Sets the name of the exchange where messages will be taken from.
    /// </summary>
    public ExchangeBindingOperationBuilder SetSource(string source)
    {
        _source = source;
        return this;
    }

    /// <summary>
    /// Sets the routing key, only those messages that are published in source exchange with this key will get into the destination exchange.
    /// The default value is an empty string.
    /// </summary>
    public ExchangeBindingOperationBuilder SetRoutingKey(string routingKey)
    {
        _routingKey = routingKey;
        return this;
    }

    /// <summary>
    /// Sets the binding arguments.
    /// </summary>
    public ExchangeBindingOperationBuilder SetArguments(IDictionary<string, object> arguments)
    {
        _arguments = arguments;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal ExchangeBindingOperation Build()
    {
        return new ExchangeBindingOperation(_destination, _source, _routingKey, _arguments);
    }
}