using Rabbiter.Builders.Results;
using Rabbiter.Connections;
using Rabbiter.Consumers.Configurations;
using Rabbiter.Loggers;

namespace Rabbiter.Consumers;

/// <summary>
/// Represents a message consumer.
/// </summary>
internal class Consumer : IConsumer
{
    private readonly IConnectionHolder _connectionHolder;
    private readonly IConsumerObjectConfigurator _configurator;
    private readonly IConsumerLogger _logger;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the class <see cref="Consumer"/>.
    /// </summary>
    public Consumer(IConnectionHolder connectionHolder, IConsumerObjectConfigurator configurator, IServiceProvider serviceProvider, IConsumerLogger logger)
    {
        _connectionHolder = connectionHolder;
        _configurator = configurator;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Subscribes to the specified messages and starts processing.
    /// </summary>
    /// <param name="instance">The instance for which to start consuming messages.</param>
    /// <param name="cancellationToken">Processing cancellation token.</param>
    public void Start(InstanceBuildResult instance, CancellationToken cancellationToken)
    {
        try
        {
            StartCore(instance, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogCreatingConsumerError(instance, exception);
        }
    }

    private void StartCore(InstanceBuildResult instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (instance.Consumer is null)
        {
            return;
        }

        if (instance.Consumer.Items.Count == 0)
        {
            return;
        }

        /*
         * It is important to keep only one model for all subscriptions, since each model has its own working pool, within which threads can be created.
         * We need one pool, which all subscriptions share among themselves.
         */
        var connection = _connectionHolder.GetOrCreateConnection(instance);
        var model = connection.CreateModel();

        foreach (var subscription in instance.Consumer.Items)
        {
            _logger.LogCreatingConsumerStart(subscription);

            try
            {
                var queueName = _configurator.SetupAndGetQueueName(model, subscription, instance.Consumer.ConsumerConfig);
                var listener = new QueueListener(subscription, _serviceProvider, _logger);
                listener.Listen(model, queueName, subscription.Name, cancellationToken);
                _logger.LogCreatingConsumerSuccess(subscription);
            }
            catch (Exception e)
            {
                _logger.LogCreatingConsumerError(subscription, e);
            }
        }
    }
}