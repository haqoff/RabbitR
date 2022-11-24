using Rabbiter.Messages;
using Rabbiter.Builders.Instances.Operations;

namespace Rabbiter.Producers;

/// <summary>
/// Provides methods for publishing messages.
/// </summary>
public interface IEventBusPublisher
{
    /// <summary>
    /// Enqueues a message to the queue of the specified instance.
    /// Note: A queue must be created before the operation.
    /// You can declare by calling <see cref="InitOperationBuilder.DeclareQueue"/> when adding instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    Task EnqueueAsync<T>(string instanceName, string queueName, T message) where T : IEventBusMessage;

    /// <summary>
    /// Enqueues a message to the queue of the default instance.
    /// Note: A queue must be created before the operation.
    /// You can declare by calling <see cref="InitOperationBuilder.DeclareQueue"/> when adding instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    Task EnqueueAsync<T>(string queueName, T message) where T : IEventBusMessage;

    /// <summary>
    /// Publishes a message to the exchange of the specified instance.
    /// Note: An exchange must be created before the operation.
    /// You can declare by calling <see cref="InitOperationBuilder.DeclareExchange"/> when adding instance.
    /// However, if you are publishing a message that you are consuming from the same application,
    /// you do not need to take any further action, because by the time of publication, all objects for consumption will be created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    /// <param name="routingKey">Routing key.</param>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    Task PublishToExchangeAsync<T>(string instanceName, string exchangeName, T message, string routingKey = "") where T : IEventBusMessage;

    /// <summary>
    /// Publishes a message to the exchange of the default instance.
    /// Note: An exchange must be created before the operation.
    /// You can declare by calling <see cref="InitOperationBuilder.DeclareExchange"/> when adding instance.
    /// However, if you are publishing a message that you are consuming from the same application,
    /// you do not need to take any further action, because by the time of publication, all objects for consumption will be created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    /// <param name="routingKey">Routing key.</param>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    Task PublishToExchangeAsync<T>(string exchangeName, T message, string routingKey = "") where T : IEventBusMessage;
}