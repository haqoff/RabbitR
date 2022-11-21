using Rabbiter.Builders.Results;
using Rabbiter.Consumers.Subscriptions;
using RabbitMQ.Client;

namespace Rabbiter.Loggers;

/// <summary>
/// Provides methods for logging a consumer state.
/// </summary>
internal interface IConsumerLogger
{
    /// <summary>
    /// Indicates that a new message has been received.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="content">Message content.</param>
    void LogMessageReceived(string queueName, Type messageType, ulong deliveryTag, ReadOnlySpan<byte> content);

    /// <summary>
    /// Indicates that the received message was processed successfully.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    void LogMessageHandlingSuccess(string queueName, Type messageType, ulong deliveryTag);

    /// <summary>
    /// Indicates that an error occurred while processing the message and reprocessing is not possible. Thus the message is lost.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="exception">Exception during processing.</param>
    /// <param name="republishedCount">The number of times this message has been published for reprocessing.</param>
    /// <param name="maxRetryCount">The maximum possible number of republishing.</param>
    void LogMessageHandlingCriticalError(string queueName, Type messageType, ulong deliveryTag, Exception exception, int republishedCount, int maxRetryCount);

    /// <summary>
    /// Indicates that an error occurred while processing the message, but reprocessing is possible. So the message will be processed again later.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="exception">Exception during processing.</param>
    /// <param name="republishedCount">The number of times this message has been published for reprocessing.</param>
    /// <param name="maxRetryCount">The maximum possible number of republishing.</param>
    void LogMessageHandlingErrorWithRetry(string queueName, Type messageType, ulong deliveryTag, Exception exception, int republishedCount, int maxRetryCount);

    /// <summary>
    /// Indicates that a new message consumer will be created.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    void LogCreatingConsumerStart(ISubscription subscription);

    /// <summary>
    /// Indicates that an error occurred while creating a new consumer for specific subscription.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    /// <param name="exception">Exception.</param>
    void LogCreatingConsumerError(ISubscription subscription, Exception exception);

    /// <summary>
    /// Indicates that an error occurred while creating a new consumer for instance.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="exception">Exception.</param>
    void LogCreatingConsumerError(InstanceBuildResult instance, Exception exception);

    /// <summary>
    /// Indicates that a new message consumer was successfully created.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    void LogCreatingConsumerSuccess(ISubscription subscription);

    /// <summary>
    /// Indicates that a cancellation has occurred for the consumer.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    void LogConsumerCancelled(string queueName, string subscriptionName);

    /// <summary>
    /// Indicates that the consumer was successfully registered.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    void LogConsumerRegistered(string queueName, string subscriptionName);

    /// <summary>
    /// Indicates that the consumer has been destroyed.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="shutdownEventArgs">Information about the destruction.</param>
    void LogConsumerShutdown(string queueName, string subscriptionName, ShutdownEventArgs shutdownEventArgs);

    /// <summary>
    /// Indicates that the consumer performed is no longer registered.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    void LogConsumerUnregistered(string queueName, string subscriptionName);
}