using Rabbiter.Consumers.Configurations;
using Rabbiter.Consumers.Handlers;
using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Messages;

namespace Rabbiter.Builders.Instances.Consumers;

/// <summary>
/// Represents the consumer builder, configuring subscriptions.
/// </summary>
public class ConsumerBuilder
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly List<ISubscription> _items = new();

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConsumerBuilder"/>.
    /// </summary>
    internal ConsumerBuilder(ConsumerConfig consumerConfig)
    {
        _consumerConfig = consumerConfig;
    }

    /// <summary>
    /// Subscribes to the specified exchange.
    /// Declaring the specified exchange and also creates an associated queue from which to consume.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="name">Exchange name.</param>
    /// <param name="actionBuilder">An action that configures the subscription.</param>
    public ConsumerBuilder SubscribeExchange<TMessage, THandler>(string name, Action<ExchangeSubscriptionBuilder<TMessage>>? actionBuilder = null)
        where THandler : class, IEventBusMessageHandler<TMessage>
        where TMessage : class, IEventBusMessage
    {
        var builder = new ExchangeSubscriptionBuilder<TMessage>(name, typeof(THandler));
        actionBuilder?.Invoke(builder);

        var result = builder.Build();
        _items.Add(result);

        return this;
    }

    /// <summary>
    /// Subscribes to the specified queue.
    /// The queue with the specified name will be created.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <typeparam name="THandler">Handler type.</typeparam>
    /// <param name="name">Queue name.</param>
    /// <param name="actionBuilder">An action that configures the subscription.</param>
    public ConsumerBuilder SubscribeQueue<TMessage, THandler>(string name, Action<QueueSubscriptionBuilder<TMessage>>? actionBuilder = null)
        where THandler : class, IEventBusMessageHandler<TMessage>
        where TMessage : class, IEventBusMessage
    {
        var builder = new QueueSubscriptionBuilder<TMessage>(name, typeof(THandler));
        actionBuilder?.Invoke(builder);

        var result = builder.Build();
        _items.Add(result);

        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal ConsumerBuildResult Build()
    {
        return new ConsumerBuildResult(_consumerConfig, _items);
    }
}