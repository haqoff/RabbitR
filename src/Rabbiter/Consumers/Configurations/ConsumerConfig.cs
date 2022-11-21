using System.ComponentModel.DataAnnotations;

namespace Rabbiter.Consumers.Configurations;

/// <summary>
/// Represents the consumer config.
/// </summary>
public class ConsumerConfig
{
    /// <summary>
    /// The name of the consumer group within which messages will be consumed.
    /// If several services consume from one exchange within the same consumer group, messages will be distributed between them.
    /// In addition, each consumer group will receive its own copy of the message within the same exchange.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "The consumer group must be specified.")]
    public string ConsumerGroup { get; set; } = null!;

    /// <summary>
    /// The maximum number of threads that can be allocated to process all incoming messages.
    /// Note that each instance will have its own thread pool, limited by this value.
    /// Optional: For each subscription, you can decrease this value to limit parallel processing within a single subscription.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Max degree of parallelism must be greater than 0.")]
    public int MaxDegreeOfParallelism { get; set; }
}