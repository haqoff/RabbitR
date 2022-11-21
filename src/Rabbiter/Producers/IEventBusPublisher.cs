using Rabbiter.Messages;

namespace Rabbiter.Producers;

/// <summary>
/// Provides methods for publishing messages.
/// </summary>
public interface IEventBusPublisher
{
    /// <summary>
    /// Enqueues a message to the queue of the specified instance.
    /// Precreates the queue if it hasn't already been created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    Task EnqueueAsync<T>(string instanceName, string queueName, T message) where T : IEventBusMessage;

    /// <summary>
    /// Enqueues a message to the queue of the default instance.
    /// Precreates the queue if it hasn't already been created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    Task EnqueueAsync<T>(string queueName, T message) where T : IEventBusMessage;

    /// <summary>
    /// Publishes a message to the exchange of the specified instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    Task PublishToExchangeAsync<T>(string instanceName, string exchangeName, T message) where T : IEventBusMessage;

    /// <summary>
    /// Publishes a message to the exchange of the default instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    Task PublishToExchangeAsync<T>(string exchangeName, T message) where T : IEventBusMessage;
}