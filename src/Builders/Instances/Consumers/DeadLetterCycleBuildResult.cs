using RabbitR.Builders.Instances.Operations.Results;

namespace RabbitR.Builders.Instances.Consumers;

/// <summary>
/// Represents the result of building dead letter mechanism.
/// </summary>
internal class DeadLetterCycleBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="DeadLetterCycleBuildResult"/>.
    /// </summary>
    internal DeadLetterCycleBuildResult(
        TimeSpan delay,
        int maxRetryCount,
        ExchangeDeclarationOperation? customStartExchange,
        ExchangeDeclarationOperation? customEndExchange,
        QueueDeclarationOperation? customQueue,
        QueueBindingOperation? customStartQueueBinding,
        QueueBindingOperation? customMainQueueBinding
    )
    {
        if (delay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Dead letter processing delay must be greater than 0.");
        }

        if (maxRetryCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetryCount), "The maximum number of processing attempts must be greater than 0.");
        }

        Delay = delay;
        MaxRetryCount = maxRetryCount;
        CustomStartExchange = customStartExchange;
        CustomEndExchange = customEndExchange;
        CustomQueue = customQueue;
        CustomStartQueueBinding = customStartQueueBinding;
        CustomMainQueueBinding = customMainQueueBinding;
    }

    /// <summary>
    /// The delay before the message can be republished and processed.
    /// </summary>
    internal TimeSpan Delay { get; }

    /// <summary>
    /// The maximum number of attempts to process the message again.
    /// </summary>
    internal int MaxRetryCount { get; }

    /// <summary>
    /// Custom Start exchange configuration.
    /// </summary>
    internal ExchangeDeclarationOperation? CustomStartExchange { get; }

    /// <summary>
    /// Custom End exchange configuration.
    /// </summary>
    internal ExchangeDeclarationOperation? CustomEndExchange { get; }

    /// <summary>
    /// Custom error queue configuration.
    /// </summary>
    internal QueueDeclarationOperation? CustomQueue { get; }

    /// <summary>
    /// Custom binding between Start exchange and error queue.
    /// </summary>
    internal QueueBindingOperation? CustomStartQueueBinding { get; }

    /// <summary>
    /// Custom binding between End exchange and main queue.
    /// </summary>
    internal QueueBindingOperation? CustomMainQueueBinding { get; }
}