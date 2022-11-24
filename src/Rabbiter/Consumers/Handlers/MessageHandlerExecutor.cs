using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Loggers;
using Rabbiter.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbiter.Consumers.Handlers;

/// <summary>
/// Provides a mechanism for running a message handler.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
internal class MessageHandlerExecutor<TMessage> : IMessageHandlerExecutor where TMessage : IEventBusMessage
{
    /// <summary>
    /// Deserializes the message, then calls the message handler.
    /// </summary>
    public async Task ExecuteAsync(string queueName, ISubscription item, IServiceProvider provider, IConsumerLogger logger, IModel model, BasicDeliverEventArgs @event, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        logger.LogMessageReceived(queueName, item.MessageType, @event.DeliveryTag, @event.Body.Span);

        try
        {
            using var scope = provider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventBusMessageHandler<TMessage>>();
            var message = JsonSerializer.Deserialize<TMessage>(@event.Body.Span)!;
            await handler.HandleAsync(message, cancellationToken);
            model.BasicAck(@event.DeliveryTag, false);
            logger.LogMessageHandlingSuccess(queueName, item.MessageType, @event.DeliveryTag);
        }
        catch (Exception e)
        {
            var (canRetry, attemptsMade, maxRetryCount) = GetRetryInfo(@event.BasicProperties, item);
            if (canRetry)
            {
                model.BasicReject(@event.DeliveryTag, false);
                logger.LogMessageHandlingErrorWithRetry(queueName, item.MessageType, @event.DeliveryTag, e, attemptsMade, maxRetryCount);
            }
            else
            {
                model.BasicAck(@event.DeliveryTag, false);
                logger.LogMessageHandlingCriticalError(queueName, item.MessageType, @event.DeliveryTag, e, attemptsMade, maxRetryCount);
            }
        }
    }

    private static (bool canRetry, int attemptsMade, int maxRetryCount) GetRetryInfo(IBasicProperties properties, ISubscription subscription)
    {
        if (subscription is not ExchangeSubscription exchangeSubscription || exchangeSubscription.DeadLetterCycle is null) return (false, 0, 0);

        var attemptsMade = GetAttemptsMade(properties);
        var maxRetryCount = exchangeSubscription.DeadLetterCycle.MaxRetryCount;
        return (attemptsMade < maxRetryCount, attemptsMade, maxRetryCount);
    }

    private static int GetAttemptsMade(IBasicProperties properties)
    {
        if (properties.Headers is null || !properties.Headers.ContainsKey("x-death"))
        {
            return 0;
        }

        var deathProperties = (List<object>) properties.Headers["x-death"];
        var lastRetry = (Dictionary<string, object>) deathProperties[0];
        var count = lastRetry["count"];

        // we don't need store long
        return (int) (long) count;
    }
}