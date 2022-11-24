using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitR.Consumers.Subscriptions;
using RabbitR.Loggers;

namespace RabbitR.Consumers.Handlers;

/// <summary>
/// Provides a method that invokes a message handler.
/// </summary>
internal interface IMessageHandlerExecutor
{
    /// <summary>
    /// Deserializes the message, then calls the message handler.
    /// </summary>
    Task ExecuteAsync(
        string queueName,
        ISubscription item,
        IServiceProvider provider,
        IConsumerLogger logger,
        IModel model,
        BasicDeliverEventArgs @event,
        CancellationToken cancellationToken
    );
}