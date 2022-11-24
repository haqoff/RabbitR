using Rabbiter.Consumers.Subscriptions;
using RabbitMQ.Client;

namespace Rabbiter.Consumers.Configurations;

/// <summary>
/// Provides a mechanism for creating and configuring objects (queues, exchanges).
/// </summary>
internal class ConsumerObjectСonfigurator : IConsumerObjectConfigurator
{
    /// <summary>
    /// Creates objects (queues, exchanges) and configures them, returning the name of the final queue from which to consume messages.
    /// </summary>
    public string SetupAndGetQueueName(IModel model, ISubscription subscription, ConsumerConfig config)
    {
        return subscription switch
        {
            ExchangeSubscription exchangeSubscription => SetupExchangeConsuming(exchangeSubscription, model, config),
            QueueSubscription queueSubscription => SetupQueueConsuming(queueSubscription, model),
            _ => throw new ArgumentOutOfRangeException(nameof(subscription))
        };
    }

    private static string SetupQueueConsuming(QueueSubscription queueSubscription, IModel model)
    {
        var mainQueueName = queueSubscription.Name;
        model.QueueDeclare(queue: mainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        return mainQueueName;
    }

    private static string SetupExchangeConsuming(ExchangeSubscription sub, IModel model, ConsumerConfig config)
    {
        var mainQueueName = sub.CustomQueue?.Queue ?? ConsumerObjectNameHelper.GetConsumerGroupMainQueue(config.ConsumerGroup, sub.Name);
        model.ExchangeDeclare(
            exchange: sub.Name,
            type: sub.CustomExchange?.Type ?? "fanout",
            durable: sub.CustomExchange?.Durable ?? true,
            autoDelete: sub.CustomExchange?.AutoDelete ?? false,
            arguments: sub.CustomExchange?.Arguments
        );

        var mainQueueArgs = new Dictionary<string, object>();
        Action? afterMainQueueDeclared = SetupExchangeDeadLetter(sub, model, config, mainQueueName, mainQueueArgs);

        model.QueueDeclare(
            queue: mainQueueName,
            durable: sub.CustomQueue?.Durable ?? true,
            exclusive: sub.CustomQueue?.Exclusive ?? false,
            autoDelete: sub.CustomQueue?.AutoDelete ?? false,
            arguments: Merge(mainQueueArgs, sub.CustomQueue?.Arguments));

        afterMainQueueDeclared?.Invoke();

        model.QueueBind(
            queue: mainQueueName,
            exchange: sub.Name,
            routingKey: sub.CustomBinding?.RoutingKey ?? string.Empty,
            arguments: sub.CustomBinding?.Arguments
        );

        return mainQueueName;
    }

    private static Action? SetupExchangeDeadLetter(ExchangeSubscription sub, IModel model, ConsumerConfig config, string mainQueueName, IDictionary<string, object> mainQueueArgs)
    {
        if (sub.DeadLetterCycle is null)
        {
            return null;
        }

        var deadLetterStartExchangeName = sub.DeadLetterCycle.CustomStartExchange?.Exchange
                                          ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterStartExchange(config.ConsumerGroup, sub.Name);
        var deadLetterEndExchangeName = sub.DeadLetterCycle.CustomEndExchange?.Exchange
                                        ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterEndExchange(config.ConsumerGroup, sub.Name);
        var deadLetterQueueName = sub.DeadLetterCycle.CustomQueue?.Queue
                                  ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterQueue(config.ConsumerGroup, sub.Name);

        // declare Start exchange
        model.ExchangeDeclare(
            exchange: deadLetterStartExchangeName,
            type: sub.DeadLetterCycle.CustomStartExchange?.Type ?? "fanout",
            durable: sub.DeadLetterCycle.CustomStartExchange?.Durable ?? true,
            autoDelete: sub.DeadLetterCycle.CustomStartExchange?.AutoDelete ?? false,
            arguments: sub.DeadLetterCycle.CustomStartExchange?.Arguments
        );

        // declare End exchange
        model.ExchangeDeclare(
            exchange: deadLetterEndExchangeName,
            type: sub.DeadLetterCycle.CustomEndExchange?.Type ?? "fanout",
            durable: sub.DeadLetterCycle.CustomEndExchange?.Durable ?? true,
            autoDelete: sub.DeadLetterCycle.CustomEndExchange?.AutoDelete ?? false,
            arguments: sub.DeadLetterCycle.CustomEndExchange?.Arguments
        );

        // declare error queue
        model.QueueDeclare(queue: deadLetterQueueName,
            durable: sub.DeadLetterCycle.CustomQueue?.Durable ?? true,
            exclusive: sub.DeadLetterCycle.CustomQueue?.Exclusive ?? false,
            autoDelete: sub.DeadLetterCycle.CustomQueue?.AutoDelete ?? false,
            arguments: Merge(new Dictionary<string, object>
            {
                {"x-message-ttl", (int) sub.DeadLetterCycle.Delay.TotalMilliseconds},
                {"x-dead-letter-exchange", deadLetterEndExchangeName},
            }, sub.DeadLetterCycle.CustomQueue?.Arguments));

        // bind error queue to Start exchange
        model.QueueBind(
            queue: deadLetterQueueName,
            exchange: deadLetterStartExchangeName,
            routingKey: sub.DeadLetterCycle.CustomStartQueueBinding?.RoutingKey ?? string.Empty,
            arguments: sub.DeadLetterCycle?.CustomStartQueueBinding?.Arguments
        );

        // bind main queue to End exchange
        void AfterMainQueueDeclared()
        {
            model.QueueBind(queue: mainQueueName, exchange: deadLetterEndExchangeName, routingKey: sub.DeadLetterCycle!.CustomMainQueueBinding?.RoutingKey ?? string.Empty,
                arguments: sub.DeadLetterCycle!.CustomMainQueueBinding?.Arguments);
        }

        mainQueueArgs.Add("x-dead-letter-exchange", deadLetterStartExchangeName);
        return AfterMainQueueDeclared;
    }

    private static Dictionary<string, object> Merge(Dictionary<string, object> defaultDict, IDictionary<string, object>? customDict)
    {
        if (customDict is null)
        {
            return defaultDict;
        }

        foreach (var (key, value) in customDict)
        {
            defaultDict[key] = value;
        }

        return defaultDict;
    }
}