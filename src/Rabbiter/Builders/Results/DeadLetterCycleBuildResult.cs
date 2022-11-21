namespace Rabbiter.Builders.Results;

/// <summary>
/// Represents the result of building dead letter mechanism.
/// </summary>
internal class DeadLetterCycleBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="DeadLetterCycleBuildResult"/>.
    /// </summary>
    internal DeadLetterCycleBuildResult(TimeSpan delay, int maxRetryCount, string? customStartExchangeName, string? customEndExchangeName, string? customQueueName)
    {
        if (delay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Dead letter processing delay must be greater than 0.");
        }

        if (maxRetryCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRetryCount), "The maximum number of processing attempts must be greater than 0.");
        }

        if (customStartExchangeName is not null && string.IsNullOrWhiteSpace(customStartExchangeName))
        {
            throw new ArgumentException("Custom start exchange name cannot be empty string.", nameof(customStartExchangeName));
        }

        if (customEndExchangeName is not null && string.IsNullOrWhiteSpace(customEndExchangeName))
        {
            throw new ArgumentException("Custom end exchange name cannot be empty string.", nameof(customEndExchangeName));
        }

        if (customQueueName is not null && string.IsNullOrWhiteSpace(customQueueName))
        {
            throw new ArgumentException("Custom queue name cannot be empty string.", nameof(customQueueName));
        }

        Delay = delay;
        MaxRetryCount = maxRetryCount;
        CustomStartExchangeName = customStartExchangeName;
        CustomEndExchangeName = customEndExchangeName;
        CustomQueueName = customQueueName;
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
    /// Custom name of the exchange, which will receive messages processed with an error.
    /// </summary>
    internal string? CustomStartExchangeName { get; }

    /// <summary>
    /// Custom name of the exchange, which will receive messages from the error queue <see cref="CustomQueueName"/> after a delay with value <see cref="Delay"/>.
    /// </summary>
    internal string? CustomEndExchangeName { get; }

    /// <summary>
    /// Custom name of the queue that will be associated with the exchange <see cref="CustomStartExchangeName"/>, in which error messages will be added.
    /// </summary>
    internal string? CustomQueueName { get; }
}