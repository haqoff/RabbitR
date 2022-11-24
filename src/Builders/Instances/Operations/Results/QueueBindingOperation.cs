namespace RabbitR.Builders.Instances.Operations.Results;

/// <summary>
/// Represents a queue binding operation.
/// </summary>
/// <param name="Queue">Queue name.</param>
/// <param name="Exchange">Exchange name.</param>
/// <param name="RoutingKey">Routing key.</param>
/// <param name="Arguments">Arguments.</param>
internal record QueueBindingOperation(string? Queue, string? Exchange, string RoutingKey, IDictionary<string, object>? Arguments);