using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Loggers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbiter.Consumers.Handlers;

/// <summary>
/// Provides a method that invokes a message handler.
/// </summary>
internal interface IMessageHandlerExecutor
{
    /// <summary>
    /// Deserializes the message, then calls the message handler.
    /// </summary>
    Task ExecuteAsync(
        ISubscription item,
        IServiceProvider provider,
        IConsumerLogger logger,
        IModel model,
        BasicDeliverEventArgs @event,
        CancellationToken cancellationToken
    );
}