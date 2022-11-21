using System.Text;
using Microsoft.Extensions.Logging;
using Rabbiter.Builders.Results;
using Rabbiter.Consumers.Subscriptions;
using RabbitMQ.Client;

namespace Rabbiter.Loggers;

/// <summary>
/// Represents a mechanism for logging consumers.
/// </summary>
internal class ConsumerLogger : IConsumerLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConsumerLogger"/>.
    /// </summary>
    public ConsumerLogger(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger(LoggerNamespace.Consumer);
    }

    /// <summary>
    /// Indicates that a new message has been received.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="content">Message content.</param>
    public void LogMessageReceived(string queueName, Type messageType, ulong deliveryTag, ReadOnlySpan<byte> content)
    {
        // TODO: enrich log by content instead write it to message directly
        var str = Encoding.UTF8.GetString(content);
        _logger.LogInformation("New message {messageName} received from queue {queueName} with tag {deliveryTag}. Content is: {content}", messageType.Name, queueName, deliveryTag, str);
    }

    /// <summary>
    /// Indicates that the received message was processed successfully.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    public void LogMessageHandlingSuccess(string queueName, Type messageType, ulong deliveryTag)
    {
        _logger.LogInformation("Message {messageName} was successfully handled for queue {queueName} with tag {deliveryTag}.", messageType.Name, queueName, deliveryTag);
    }

    /// <summary>
    /// Indicates that an error occurred while processing the message and reprocessing is not possible. Thus the message is lost.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="exception">Exception during processing.</param>
    /// <param name="republishedCount">The number of times this message has been published for reprocessing.</param>
    /// <param name="maxRetryCount">The maximum possible number of republishing.</param>
    public void LogMessageHandlingCriticalError(string queueName, Type messageType, ulong deliveryTag, Exception exception, int republishedCount, int maxRetryCount)
    {
        _logger.LogCritical(exception,
            "An error occurred while processing message {messageName} for queue {queueName} with tag {deliveryTag}. This message has been republished {republishedCount} times. Never succeeded in processing it successfully, the message will not be republished again.",
            messageType.Name, queueName, deliveryTag, republishedCount);
    }

    /// <summary>
    /// Indicates that an error occurred while processing the message, but reprocessing is possible. So the message will be processed again later.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="messageType">Message type.</param>
    /// <param name="deliveryTag">Delivery tag of the message.</param>
    /// <param name="exception">Exception during processing.</param>
    /// <param name="republishedCount">The number of times this message has been published for reprocessing.</param>
    /// <param name="maxRetryCount">The maximum possible number of republishing.</param>
    public void LogMessageHandlingErrorWithRetry(string queueName, Type messageType, ulong deliveryTag, Exception exception, int republishedCount, int maxRetryCount)
    {
        _logger.LogWarning(exception,
            "An error occurred while processing message {messageName} for queue {queueName} with tag {deliveryTag}. The message will be republished. There are {attemptLeft} attempts left.",
            messageType.Name, queueName, deliveryTag, maxRetryCount - republishedCount);
    }

    /// <summary>
    /// Indicates that a new message consumer will be created.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    public void LogCreatingConsumerStart(ISubscription subscription)
    {
        _logger.LogInformation("Start creating a consumer for the message {messageName} consumed from {objectName}.", subscription.MessageType.Name, subscription.Name);
    }

    /// <summary>
    /// Indicates that an error occurred while creating a new consumer.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    /// <param name="exception">Exception.</param>
    public void LogCreatingConsumerError(ISubscription subscription, Exception exception)
    {
        _logger.LogError(exception, "An error occurred while creating a consumer for message {messageName} consumed from {objectName}.", subscription.MessageType.Name, subscription.Name);
    }

    /// <summary>
    /// Indicates that an error occurred while creating a new consumer for instance.
    /// </summary>
    /// <param name="instance">Instance.</param>
    /// <param name="exception">Exception.</param>
    public void LogCreatingConsumerError(InstanceBuildResult instance, Exception exception)
    {
        _logger.LogError(exception, "An error occurred while creating a consumer for instance {instanceName}.", instance.Name);
    }

    /// <summary>
    /// Indicates that a new message consumer was successfully created.
    /// </summary>
    /// <param name="subscription">Subscription.</param>
    public void LogCreatingConsumerSuccess(ISubscription subscription)
    {
        _logger.LogInformation("The consumer for message {messageName} consumed from {objectName} was created successfully.", subscription.MessageType.Name, subscription.Name);
    }

    /// <summary>
    /// Indicates that a cancellation has occurred for the consumer.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    public void LogConsumerCancelled(string queueName, string subscriptionName)
    {
        _logger.LogWarning("The consumer for queue {queueName} (subscription: {subscription}) has been cancelled.", queueName, subscriptionName);
    }

    /// <summary>
    /// Indicates that the consumer was successfully registered.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    public void LogConsumerRegistered(string queueName, string subscriptionName)
    {
        _logger.LogInformation("The consumer for queue {queueName} (subscription: {subscription}) has been successfully registered.", queueName, subscriptionName);
    }

    /// <summary>
    /// Indicates that the consumer has been destroyed.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="shutdownEventArgs">Information about the destruction.</param>
    public void LogConsumerShutdown(string queueName, string subscriptionName, ShutdownEventArgs shutdownEventArgs)
    {
        _logger.LogWarning("The consumer for queue {queueName} (subscription: {subscription}) has been shut down. Info: {info}.", queueName, subscriptionName, shutdownEventArgs.ToString());
    }

    /// <summary>
    /// Indicates that the consumer performed is no longer registered.
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    public void LogConsumerUnregistered(string queueName, string subscriptionName)
    {
        _logger.LogWarning("The consumer for queue {queueName} (subscription: {subscription}) has been unregistered.", queueName, subscriptionName);
    }
}