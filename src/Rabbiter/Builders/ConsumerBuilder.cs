using Rabbiter.Builders.Results;
using Rabbiter.Consumers.Configurations;
using Rabbiter.Consumers.Subscriptions;
using Rabbiter.Messages;

namespace Rabbiter.Builders;

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
    /// </summary>
    /// <typeparam name="TMessage">Message.</typeparam>
    /// <param name="name">Exchange name.</param>
    /// <param name="actionBuilder">An action that configures the subscription.</param>
    public ConsumerBuilder SubscribeExchange<TMessage>(string name, Action<ExchangeSubscriptionBuilder<TMessage>>? actionBuilder = null) where TMessage : IEventBusMessage
    {
        var builder = new ExchangeSubscriptionBuilder<TMessage>(name);
        actionBuilder?.Invoke(builder);

        var result = builder.Build();
        _items.Add(result);

        return this;
    }

    /// <summary>
    /// Subscribes to the specified queue.
    /// </summary>
    /// <typeparam name="TMessage">Message.</typeparam>
    /// <param name="name">Queue name.</param>
    /// <param name="actionBuilder">An action that configures the subscription.</param>
    public ConsumerBuilder SubscribeQueue<TMessage>(string name, Action<QueueSubscriptionBuilder<TMessage>>? actionBuilder = null) where TMessage : IEventBusMessage
    {
        var builder = new QueueSubscriptionBuilder<TMessage>(name);
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