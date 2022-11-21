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

    private static string SetupExchangeConsuming(ExchangeSubscription exchangeSubscription, IModel model, ConsumerConfig config)
    {
        var mainQueueName = exchangeSubscription.CustomQueueName ?? ConsumerObjectNameHelper.GetConsumerGroupMainQueue(config.ConsumerGroup, exchangeSubscription.Name);
        model.ExchangeDeclare(exchange: exchangeSubscription.Name, type: "fanout", durable: true, autoDelete: false, arguments: null);

        var mainQueueArgs = new Dictionary<string, object>();
        Action? afterMainQueueDeclared = null;

        if (exchangeSubscription.DeadLetterCycle is not null)
        {
            var deadLetterStartExchangeName = exchangeSubscription.DeadLetterCycle.CustomStartExchangeName
                                              ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterStartExchange(config.ConsumerGroup, exchangeSubscription.Name);
            var deadLetterEndExchangeName = exchangeSubscription.DeadLetterCycle.CustomEndExchangeName
                                            ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterEndExchange(config.ConsumerGroup, exchangeSubscription.Name);
            var deadLetterQueueName = exchangeSubscription.DeadLetterCycle.CustomQueueName
                                      ?? ConsumerObjectNameHelper.GetConsumerGroupDeadLetterQueue(config.ConsumerGroup, exchangeSubscription.Name);

            model.ExchangeDeclare(exchange: deadLetterStartExchangeName, "fanout", durable: true, autoDelete: false, arguments: null);
            model.ExchangeDeclare(exchange: deadLetterEndExchangeName, "fanout", durable: true, autoDelete: false, arguments: null);

            model.QueueDeclare(queue: deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    {"x-message-ttl", (int) exchangeSubscription.DeadLetterCycle.Delay.TotalMilliseconds},
                    {"x-dead-letter-exchange", deadLetterEndExchangeName},
                });

            model.QueueBind(queue: deadLetterQueueName, exchange: deadLetterStartExchangeName, string.Empty, arguments: null);

            afterMainQueueDeclared = () => { model.QueueBind(queue: mainQueueName, exchange: deadLetterEndExchangeName, string.Empty, arguments: null); };

            mainQueueArgs.Add("x-dead-letter-exchange", deadLetterStartExchangeName);
        }

        model.QueueDeclare(queue: mainQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: mainQueueArgs);

        afterMainQueueDeclared?.Invoke();

        model.QueueBind(queue: mainQueueName, exchange: exchangeSubscription.Name, string.Empty, arguments: null);
        return mainQueueName;
    }
}