using System.Text.Json;
using RabbitR.Builders.Instances.Operations;
using RabbitR.Connections;
using RabbitR.Loggers;
using RabbitR.Messages;

namespace RabbitR.Producers;

/// <summary>
/// Represents a message publishing mechanism.
/// </summary>
internal class EventBusPublisher : IEventBusPublisher
{
    private readonly IProducerModelPoolStorage _storage;
    private readonly Task _consumerCreationTask;
    private readonly IProducerLogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="EventBusPublisher"/>.
    /// </summary>
    public EventBusPublisher(IProducerModelPoolStorage storage, Task consumerCreationTask, IProducerLogger logger)
    {
        _storage = storage;
        _consumerCreationTask = consumerCreationTask;
        _logger = logger;
    }

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
    public async Task EnqueueAsync<T>(string instanceName, string queueName, T message) where T : IEventBusMessage
    {
        var pool = _storage.Get(instanceName);
        var model = await pool.TakeAsync();

        try
        {
            var body = JsonSerializer.SerializeToUtf8Bytes((object) message);

            model.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body,
                mandatory: false);

            _logger.LogMessagePublishingSuccess(instanceName, queueName, message);
        }
        catch (Exception e)
        {
            _logger.LogMessagePublishingError(e, instanceName, queueName, message);
        }
        finally
        {
            pool.Return(model);
        }
    }

    /// <summary>
    /// Enqueues a message to the queue of the default instance.
    /// Note: A queue must be created before the operation.
    /// You can declare by calling <see cref="InitOperationBuilder.DeclareQueue"/> when adding instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    public Task EnqueueAsync<T>(string queueName, T message) where T : IEventBusMessage
    {
        return EnqueueAsync(InstanceConstants.DefaultName, queueName, message);
    }

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
    public async Task PublishToExchangeAsync<T>(string instanceName, string exchangeName, T message, string routingKey = "") where T : IEventBusMessage
    {
        var pool = _storage.Get(instanceName);
        var model = await pool.TakeAsync();

        /*
         * Before publishing to the exchange, we need to make sure that all consumers have been created for the reason that we can publish to the exchange from which we consume.
         * For consumption, we create a separate queue associated with the exchange, and by the time of publication, it may not yet be created.
         * If we publish before the queue is created, the message for it will be lost.
         */
        await _consumerCreationTask;

        try
        {
            var body = JsonSerializer.SerializeToUtf8Bytes((object) message);

            model.BasicPublish(exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body,
                mandatory: false);

            _logger.LogMessagePublishingSuccess(instanceName, exchangeName, message);
        }
        catch (Exception e)
        {
            _logger.LogMessagePublishingError(e, instanceName, exchangeName, message);
        }
        finally
        {
            pool.Return(model);
        }
    }

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
    public Task PublishToExchangeAsync<T>(string exchangeName, T message, string routingKey = "") where T : IEventBusMessage
    {
        return PublishToExchangeAsync(InstanceConstants.DefaultName, exchangeName, message, routingKey);
    }
}