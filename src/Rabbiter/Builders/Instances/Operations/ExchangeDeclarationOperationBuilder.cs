using Rabbiter.Builders.Instances.Operations.Results;

namespace Rabbiter.Builders.Instances.Operations;

/// <summary>
/// Represents the builder of the exchange declaration operation.
/// </summary>
public class ExchangeDeclarationOperationBuilder : IExchangeDeclarationOperationNoNameBuilder
{
    private string? _name;
    private string _type = "fanout";
    private bool _durable = true;
    private bool _autoDelete;
    private IDictionary<string, object>? _arguments;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ExchangeDeclarationOperationBuilder"/>.
    /// </summary>
    internal ExchangeDeclarationOperationBuilder(string? name)
    {
        _name = name;
    }

    /// <summary>
    /// Sets the exchange name.
    /// </summary>
    public ExchangeDeclarationOperationBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the exchange type.
    /// The default value is fanout.
    /// </summary>
    public ExchangeDeclarationOperationBuilder SetType(string type)
    {
        _type = type;
        return this;
    }

    /// <summary>
    /// Should this exchange will survive a broker restart?
    /// The default value is true.
    /// </summary>
    public ExchangeDeclarationOperationBuilder SetDurable(bool durable)
    {
        _durable = durable;
        return this;
    }

    /// <summary>
    /// Should this exchange be auto-deleted when its last consumer (if any) unsubscribes?
    /// The default value is false.
    /// </summary>
    public ExchangeDeclarationOperationBuilder SetAutoDelete(bool autoDelete)
    {
        _autoDelete = autoDelete;
        return this;
    }

    /// <summary>
    /// Sets the exchange creation arguments.
    /// By default, no arguments are passed.
    /// </summary>
    public ExchangeDeclarationOperationBuilder SetArguments(IDictionary<string, object> arguments)
    {
        _arguments = arguments;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal ExchangeDeclarationOperation Build()
    {
        return new ExchangeDeclarationOperation(_name, _type, _durable, _autoDelete, _arguments);
    }

    IExchangeDeclarationOperationNoNameBuilder IExchangeDeclarationOperationNoNameBuilder.SetType(string type) => SetType(type);
    IExchangeDeclarationOperationNoNameBuilder IExchangeDeclarationOperationNoNameBuilder.SetDurable(bool durable) => SetDurable(durable);
    IExchangeDeclarationOperationNoNameBuilder IExchangeDeclarationOperationNoNameBuilder.SetAutoDelete(bool autoDelete) => SetAutoDelete(autoDelete);
    IExchangeDeclarationOperationNoNameBuilder IExchangeDeclarationOperationNoNameBuilder.SetArguments(IDictionary<string, object> arguments) => SetArguments(arguments);
}