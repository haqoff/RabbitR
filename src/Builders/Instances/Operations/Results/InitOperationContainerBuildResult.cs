namespace RabbitR.Builders.Instances.Operations.Results;

/// <summary>
/// Represents a container of operations that must be performed to initialize Rabbit MQ.
/// </summary>
internal class InitOperationContainerBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="InitOperationContainerBuildResult"/>.
    /// </summary>
    internal InitOperationContainerBuildResult(
        IReadOnlyList<ExchangeDeclarationOperation> exchangeDeclarationOperations,
        IReadOnlyList<QueueDeclarationOperation> queueDeclarationOperations,
        IReadOnlyList<QueueBindingOperation> queueBindingOperations,
        IReadOnlyList<ExchangeBindingOperation> exchangeBindingOperations)
    {
        ExchangeDeclarationOperations = exchangeDeclarationOperations;
        QueueDeclarationOperations = queueDeclarationOperations;
        QueueBindingOperations = queueBindingOperations;
        ExchangeBindingOperations = exchangeBindingOperations;
    }

    /// <summary>
    /// Exchange creation operations.
    /// </summary>
    internal IReadOnlyList<ExchangeDeclarationOperation> ExchangeDeclarationOperations { get; }

    /// <summary>
    /// Queue declaration operations.
    /// </summary>
    internal IReadOnlyList<QueueDeclarationOperation> QueueDeclarationOperations { get; }

    /// <summary>
    /// Queue binding operations.
    /// </summary>
    internal IReadOnlyList<QueueBindingOperation> QueueBindingOperations { get; }

    /// <summary>
    /// Exchange binding operations.
    /// </summary>
    internal IReadOnlyList<ExchangeBindingOperation> ExchangeBindingOperations { get; }
}