using System.Collections.Concurrent;
using System.Text.Json;
using Rabbiter.Connections;
using Rabbiter.Loggers;
using Rabbiter.Messages;

namespace Rabbiter.Producers;

/// <summary>
/// Represents a message publishing mechanism.
/// </summary>
internal class EventBusPublisher : IEventBusPublisher
{
    private readonly IProducerModelPoolStorage _storage;
    private readonly Task _consumerCreationTask;
    private readonly IProducerLogger _logger;
    private readonly ConcurrentDictionary<string, bool> _configuredObjects;

    /// <summary>
    /// Initializes a new instance of the class <see cref="EventBusPublisher"/>.
    /// </summary>
    public EventBusPublisher(IProducerModelPoolStorage storage, Task consumerCreationTask, IProducerLogger logger)
    {
        _storage = storage;
        _consumerCreationTask = consumerCreationTask;
        _logger = logger;
        _configuredObjects = new ConcurrentDictionary<string, bool>();
    }

    /// <summary>
    /// Enqueues a message to the queue of the specified instance.
    /// Precreates the queue if it hasn't already been created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    public async Task EnqueueAsync<T>(string instanceName, string queueName, T message) where T : IEventBusMessage
    {
        var pool = _storage.Get(instanceName);
        var model = await pool.TakeAsync();

        try
        {
            // PERF: execute the queue declaration only once to remove unnecessary network delays.
            if (!_configuredObjects.ContainsKey(queueName))
            {
                model.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _configuredObjects.TryAdd(queueName, false);
            }

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
    /// Precreates the queue if it hasn't already been created.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="queueName">Queue name.</param>
    /// <param name="message">Message.</param>
    public Task EnqueueAsync<T>(string queueName, T message) where T : IEventBusMessage
    {
        return EnqueueAsync(InstanceConstants.DefaultName, queueName, message);
    }

    /// <summary>
    /// Publishes a message to the exchange of the specified instance.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    public async Task PublishToExchangeAsync<T>(string instanceName, string exchangeName, T message) where T : IEventBusMessage
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
            // PERF: execute the exchange declaration only once to remove unnecessary network delays.
            if (!_configuredObjects.ContainsKey(exchangeName))
            {
                model.ExchangeDeclare(exchange: exchangeName,
                    durable: true,
                    autoDelete: false,
                    arguments: null,
                    type: "fanout");

                _configuredObjects.TryAdd(exchangeName, false);
            }

            var body = JsonSerializer.SerializeToUtf8Bytes((object) message);

            model.BasicPublish(exchange: exchangeName,
                routingKey: "",
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
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    /// <param name="exchangeName">Exchange name.</param>
    /// <param name="message">Message.</param>
    public Task PublishToExchangeAsync<T>(string exchangeName, T message) where T : IEventBusMessage
    {
        return PublishToExchangeAsync(InstanceConstants.DefaultName, exchangeName, message);
    }
}