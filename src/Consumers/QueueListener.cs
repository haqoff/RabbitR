using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitR.Consumers.Handlers;
using RabbitR.Consumers.Subscriptions;
using RabbitR.Loggers;

namespace RabbitR.Consumers;

/// <summary>
/// Provides a mechanism for listening to a queue and then processing them.
/// </summary>
internal class QueueListener
{
    private readonly ISubscription _subscription;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumerLogger _logger;
    private readonly IMessageHandlerExecutor _executor;

    /// <summary>
    /// Initializes a new instance of the class <see cref="QueueListener"/>.
    /// </summary>
    internal QueueListener(ISubscription subscription, IServiceProvider serviceProvider, IConsumerLogger logger)
    {
        _subscription = subscription;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _executor = (IMessageHandlerExecutor?) Activator.CreateInstance(typeof(MessageHandlerExecutor<>).MakeGenericType(subscription.MessageType))
                    ?? throw new InvalidOperationException("Cannot create ProcessingExecutor.");
    }

    /// <summary>
    /// Listens on the specified queue, and when a message is received, starts processing it.
    /// </summary>
    /// <param name="model">Channel.</param>
    /// <param name="queueName">The name of the queue to read from.</param>
    /// <param name="subscriptionName">Subscription name.</param>
    /// <param name="cancellationToken">Processing cancellation token.</param>
    public void Listen(IModel model, string queueName, string subscriptionName, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(model);
        consumer.ConsumerCancelled += (_, _) => HandleConsumerCancelledAsync(queueName, subscriptionName);
        consumer.Registered += (_, _) => HandleConsumerRegisteredAsync(queueName, subscriptionName);
        consumer.Shutdown += (_, @event) => HandleConsumerShutdownAsync(queueName, subscriptionName, @event);
        consumer.Unregistered += (_, _) => HandleConsumerUnregisteredAsync(queueName, subscriptionName);
        consumer.Received += (_, @event) => _executor.ExecuteAsync(queueName, _subscription, _serviceProvider, _logger, model, @event, cancellationToken);

        model.BasicQos(0, _subscription.MaxDegreeOfParallelism, false);
        var tag = model.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        cancellationToken.Register(() => HandleCancellation(model, tag));
    }

    private Task HandleConsumerUnregisteredAsync(string queueName, string subscriptionName)
    {
        _logger.LogConsumerUnregistered(queueName, subscriptionName);
        return Task.CompletedTask;
    }

    private Task HandleConsumerShutdownAsync(string queueName, string subscriptionName, ShutdownEventArgs e)
    {
        _logger.LogConsumerShutdown(queueName, subscriptionName, e);
        return Task.CompletedTask;
    }

    private Task HandleConsumerRegisteredAsync(string queueName, string subscriptionName)
    {
        _logger.LogConsumerRegistered(queueName, subscriptionName);
        return Task.CompletedTask;
    }

    private Task HandleConsumerCancelledAsync(string queueName, string subscriptionName)
    {
        _logger.LogConsumerCancelled(queueName, subscriptionName);
        return Task.CompletedTask;
    }

    private static void HandleCancellation(IModel model, string tag)
    {
        model.BasicCancelNoWait(tag);
    }
}