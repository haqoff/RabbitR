namespace Rabbiter.Builders.Instances.Operations.Results;

/// <summary>
/// Represents an exchange binding operation.
/// </summary>
/// <param name="Destination">Destination exchange.</param>
/// <param name="Source">Source exchange.</param>
/// <param name="RoutingKey">Routing key.</param>
/// <param name="Arguments">Arguments.</param>
internal record ExchangeBindingOperation(string? Destination, string? Source, string RoutingKey, IDictionary<string, object>? Arguments);