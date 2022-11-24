using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Operations;

/// <summary>
/// Represents a builder of operations that must be performed to initialize Rabbit MQ.
/// </summary>
public class InitOperationBuilder
{
    private readonly List<ExchangeDeclarationOperation> _exchangeCreationItems;
    private readonly List<QueueDeclarationOperation> _queueCreationItems;
    private readonly List<QueueBindingOperation> _queueBindOperations;
    private readonly List<ExchangeBindingOperation> _exchangeBindOperations;

    /// <summary>
    /// Initializes a new instance of the class <see cref="InitOperationBuilder"/>.
    /// </summary>
    internal InitOperationBuilder()
    {
        _exchangeCreationItems = new List<ExchangeDeclarationOperation>();
        _queueCreationItems = new List<QueueDeclarationOperation>();
        _queueBindOperations = new List<QueueBindingOperation>();
        _exchangeBindOperations = new List<ExchangeBindingOperation>();
    }

    /// <summary>
    /// Indicates that an exchange should be created if not already created.
    /// </summary>
    public InitOperationBuilder DeclareExchange(Action<ExchangeDeclarationOperationBuilder> action)
    {
        var builder = new ExchangeDeclarationOperationBuilder(null);
        action(builder);
        _exchangeCreationItems.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Indicates that a queue should be created if not already created.
    /// </summary>
    public InitOperationBuilder DeclareQueue(Action<QueueDeclarationOperationBuilder> action)
    {
        var builder = new QueueDeclarationOperationBuilder(null);
        action(builder);
        _queueCreationItems.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Indicates that a queue should be associated to an exchange.
    /// </summary>
    public InitOperationBuilder QueueBind(Action<QueueBindingOperationBuilder> action)
    {
        var builder = new QueueBindingOperationBuilder();
        action(builder);
        _queueBindOperations.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Indicates that a exchange should be associated to another exchange.
    /// </summary>
    public InitOperationBuilder ExchangeBind(Action<ExchangeBindingOperationBuilder> action)
    {
        var builder = new ExchangeBindingOperationBuilder();
        action(builder);
        _exchangeBindOperations.Add(builder.Build());
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal InitOperationContainerBuildResult Build()
    {
        return new InitOperationContainerBuildResult(_exchangeCreationItems, _queueCreationItems, _queueBindOperations, _exchangeBindOperations);
    }
}