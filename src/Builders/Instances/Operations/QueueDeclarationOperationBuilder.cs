using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Represents the builder of the queue declaration operation.
/// </summary>
public class QueueDeclarationOperationBuilder : IQueueDeclarationOperationNoNameBuilder
{
    private string? _name;
    private bool _durable = true;
    private bool _exclusive;
    private bool _autoDelete;
    private IDictionary<string, object>? _arguments;

    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueDeclarationOperationBuilder"/>.
    /// </summary>
    internal QueueDeclarationOperationBuilder(string? name)
    {
        _name = name;
    }

    /// <summary>
    /// Sets the queue name.
    /// </summary>
    public QueueDeclarationOperationBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Should this queue will survive a broker restart?
    /// The default value is true.
    /// </summary>
    public QueueDeclarationOperationBuilder SetDurable(bool durable)
    {
        _durable = durable;
        return this;
    }

    /// <summary>
    /// Should this queue use be limited to its declaring connection? Such a queue will be deleted when its declaring connection closes.
    /// The default value is false.
    /// </summary>
    public QueueDeclarationOperationBuilder SetExclusive(bool exclusive)
    {
        _exclusive = exclusive;
        return this;
    }

    /// <summary>
    /// Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
    /// The default value is false.
    /// </summary>
    public QueueDeclarationOperationBuilder SetAutoDelete(bool autoDelete)
    {
        _autoDelete = autoDelete;
        return this;
    }

    /// <summary>
    /// Sets the queue creation arguments.
    /// By default, no arguments are passed.
    /// </summary>
    public QueueDeclarationOperationBuilder SetArguments(IDictionary<string, object> arguments)
    {
        _arguments = arguments;
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal QueueDeclarationOperation Build()
    {
        return new QueueDeclarationOperation(_name, _durable, _exclusive, _autoDelete, _arguments);
    }

    IQueueDeclarationOperationNoNameBuilder IQueueDeclarationOperationNoNameBuilder.SetDurable(bool durable) => SetDurable(durable);
    IQueueDeclarationOperationNoNameBuilder IQueueDeclarationOperationNoNameBuilder.SetExclusive(bool exclusive) => SetExclusive(exclusive);
    IQueueDeclarationOperationNoNameBuilder IQueueDeclarationOperationNoNameBuilder.SetAutoDelete(bool autoDelete) => SetAutoDelete(autoDelete);
    IQueueDeclarationOperationNoNameBuilder IQueueDeclarationOperationNoNameBuilder.SetArguments(IDictionary<string, object> arguments) => SetArguments(arguments);
}