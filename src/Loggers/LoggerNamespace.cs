namespace RabbitR.Loggers;

/// <summary>
/// Provides constants for logging category names.
/// </summary>
internal static class LoggerNamespace
{
    private const string RabbiterBase = "Rabbiter.";

    /// <summary>
    /// Namespace for connection logging.
    /// </summary>
    internal const string Connection = RabbiterBase + "Connection";

    /// <summary>
    /// Namespace for consumer logging.
    /// </summary>
    internal const string Consumer = RabbiterBase + "Consumer";

    /// <summary>
    /// Namespace for producer logging.
    /// </summary>
    internal const string Producer = RabbiterBase + "Producer";
}