namespace RabbitR.Builders.Instances.Operations.Results;

/// <summary>
/// Represents a queue declaration operation.
/// </summary>
/// <param name="Queue">The name of the queue.</param>
/// <param name="Durable">Should this queue will survive a broker restart?</param>
/// <param name="Exclusive">Should this queue use be limited to its declaring connection? Such a queue will be deleted when its declaring connection closes.</param>
/// <param name="AutoDelete">Should this queue be auto-deleted when its last consumer (if any) unsubscribes?</param>
/// <param name="Arguments">Optional; additional exchange arguments</param>
internal record QueueDeclarationOperation(string? Queue, bool Durable, bool Exclusive, bool AutoDelete, IDictionary<string, object>? Arguments);