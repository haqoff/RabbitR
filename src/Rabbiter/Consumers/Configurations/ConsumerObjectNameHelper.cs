namespace Rabbiter.Consumers.Configurations;

/// <summary>
/// Provides helper methods for getting the default names of Rabbit MQ objects.
/// </summary>
internal static class ConsumerObjectNameHelper
{
    /// <summary>
    /// Gets the queue name within the specified consumer group to consume from the specified exchange.
    /// </summary>
    /// <remarks>
    /// This queue is associated with exchange <paramref name="exchangeName"/>, and can also be associated with exchange for reprocessing obtained by <see cref="GetConsumerGroupDeadLetterEndExchange"/>.
    /// </remarks>
    /// <param name="consumerGroup">Consumer group name.</param>
    /// <param name="exchangeName">Source exchange name.</param>
    /// <returns>Name of queue.</returns>
    internal static string GetConsumerGroupMainQueue(string consumerGroup, string exchangeName)
    {
        return exchangeName + '_' + consumerGroup + "_main_queue";
    }

    /// <summary>
    /// Gets the name of the dead letter exchange. Processed messages with an error will be added to this exchange.
    /// </summary>
    /// <param name="consumerGroup">Consumer group name.</param>
    /// <param name="exchangeName">Source exchange name.</param>
    /// <returns>Name of dead letter exchange.</returns>
    internal static string GetConsumerGroupDeadLetterStartExchange(string consumerGroup, string exchangeName)
    {
        return exchangeName + '_' + consumerGroup + "_error_exchange_start";
    }

    /// <summary>
    /// Gets the name of the exchange that will receive messages that have passed the timeout from a dead letter exchange.
    /// This exchange can be linked to the main queue (<see cref="GetConsumerGroupMainQueue"/>) to reprocess messages.
    /// </summary>
    /// <param name="consumerGroup">Consumer group name.</param>
    /// <param name="exchangeName">Source exchange name.</param>
    /// <returns>The name of the exchange from which unsuccessful messages can be processed.</returns>
    internal static string GetConsumerGroupDeadLetterEndExchange(string consumerGroup, string exchangeName)
    {
        return exchangeName + '_' + consumerGroup + "_error_exchange_end";
    }

    /// <summary>
    /// Gets the name of the queue that will be associated with the dead letter exchange (<see cref="GetConsumerGroupDeadLetterStartExchange"/>).
    /// Messages in this queue will wait for their timeout and get to the exchange for re-processing <see cref="GetConsumerGroupDeadLetterEndExchange"/>.
    /// </summary>
    /// <param name="consumerGroup">Consumer group name.</param>
    /// <param name="exchangeName">Source exchange name.</param>
    /// <returns>The name of the queue that will be associated with the dead letter exchange.</returns>
    internal static string GetConsumerGroupDeadLetterQueue(string consumerGroup, string exchangeName)
    {
        return exchangeName + '_' + consumerGroup + "_error_queue";
    }
}