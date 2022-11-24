namespace Rabbiter.Builders.Instances.Operations;

/// <summary>
/// Provides methods for building an exchange declaration operation without specifying a name.
/// </summary>
public interface IQueueDeclarationOperationNoNameBuilder
{
    /// <summary>
    /// Should this queue will survive a broker restart?
    /// The default value is true.
    /// </summary>
    IQueueDeclarationOperationNoNameBuilder SetDurable(bool durable);

    /// <summary>
    /// Should this queue use be limited to its declaring connection? Such a queue will be deleted when its declaring connection closes.
    /// The default value is false.
    /// </summary>
    IQueueDeclarationOperationNoNameBuilder SetExclusive(bool exclusive);

    /// <summary>
    /// Should this queue be auto-deleted when its last consumer (if any) unsubscribes?
    /// The default value is false.
    /// </summary>
    IQueueDeclarationOperationNoNameBuilder SetAutoDelete(bool autoDelete);

    /// <summary>
    /// Sets the queue creation arguments.
    /// By default, no arguments are passed.
    /// </summary>
    IQueueDeclarationOperationNoNameBuilder SetArguments(IDictionary<string, object> arguments);
}