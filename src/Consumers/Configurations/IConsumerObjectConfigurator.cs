using RabbitMQ.Client;
using RabbitR.Consumers.Subscriptions;

namespace RabbitR.Consumers.Configurations;

/// <summary>
/// Provides a method for setting and configuring a subscription.
/// </summary>
internal interface IConsumerObjectConfigurator
{
    /// <summary>
    /// Creates objects (queues, exchanges) and configures them, returning the name of the final queue from which to consume messages.
    /// </summary>
    string SetupAndGetQueueName(IModel model, ISubscription subscription, ConsumerConfig config);
}