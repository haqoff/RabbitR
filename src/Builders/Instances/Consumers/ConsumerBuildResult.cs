using System.ComponentModel.DataAnnotations;
using RabbitR.Consumers.Configurations;
using RabbitR.Consumers.Subscriptions;
using RabbitR.Utils;

namespace RabbitR.Builders.Instances.Consumers;

/// <summary>
/// Represents the result of building consumer.
/// </summary>
internal class ConsumerBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="ConsumerBuildResult"/>.
    /// </summary>
    internal ConsumerBuildResult(ConsumerConfig consumerConfig, IReadOnlyList<ISubscription> items)
    {
        RabbiterValidator.ThrowIfNotValid(consumerConfig);
        ThrowIfNotValid(consumerConfig, items);

        ConsumerConfig = consumerConfig;
        Items = items;
    }

    /// <summary>
    /// Consumer config.
    /// </summary>
    internal ConsumerConfig ConsumerConfig { get; }

    /// <summary>
    /// Subscriptions.
    /// </summary>
    internal IReadOnlyList<ISubscription> Items { get; }

    private static void ThrowIfNotValid(ConsumerConfig consumerConfig, IReadOnlyList<ISubscription> items)
    {
        foreach (var item in items)
        {
            if (item.MaxDegreeOfParallelism > consumerConfig.MaxDegreeOfParallelism)
            {
                throw new ValidationException(
                    "The maximum number of messages that can be processed in parallel within a single subscription cannot exceed the total number of allocated threads from the pool.");
            }
        }
    }
}