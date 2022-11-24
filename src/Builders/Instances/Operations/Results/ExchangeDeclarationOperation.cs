namespace RabbitR.Builders.Instances.Operations.Results;

/// <summary>
/// Represents an exchange declaration operation.
/// </summary>
/// <param name="Exchange">The name of the exchange.</param>
/// <param name="Type">Type of exchange.</param>
/// <param name="Durable">Should this exchange will survive a broker restart?</param>
/// <param name="AutoDelete">Should this exchange be auto-deleted when its last consumer (if any) unsubscribes?</param>
/// <param name="Arguments">Optional; additional exchange arguments</param>
internal record ExchangeDeclarationOperation(string? Exchange, string Type, bool Durable, bool AutoDelete, IDictionary<string, object>? Arguments);