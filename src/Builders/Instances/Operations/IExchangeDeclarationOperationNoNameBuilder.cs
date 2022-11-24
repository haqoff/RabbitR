namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Provides methods for building an exchange declaration operation without specifying a name.
/// </summary>
public interface IExchangeDeclarationOperationNoNameBuilder
{
    /// <summary>
    /// Sets the exchange type.
    /// The default value is fanout.
    /// </summary>
    IExchangeDeclarationOperationNoNameBuilder SetType(string type);

    /// <summary>
    /// Should this exchange will survive a broker restart?
    /// The default value is true.
    /// </summary>
    IExchangeDeclarationOperationNoNameBuilder SetDurable(bool durable);

    /// <summary>
    /// Should this exchange be auto-deleted when its last consumer (if any) unsubscribes?
    /// The default value is false.
    /// </summary>
    IExchangeDeclarationOperationNoNameBuilder SetAutoDelete(bool autoDelete);

    /// <summary>
    /// Sets the exchange creation arguments.
    /// By default, no arguments are passed.
    /// </summary>
    IExchangeDeclarationOperationNoNameBuilder SetArguments(IDictionary<string, object> arguments);
}