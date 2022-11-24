using RabbitR.Messages;

namespace RabbitR.Loggers;

/// <summary>
/// Provides methods for logging a producer state.
/// </summary>
internal interface IProducerLogger
{
    /// <summary>
    /// Indicates that the publication of the message was successful.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="objectName">Object name.</param>
    /// <param name="message">Message content.</param>
    void LogMessagePublishingSuccess<TMessage>(string instanceName, string objectName, TMessage message) where TMessage : IEventBusMessage;

    /// <summary>
    /// Indicates that an error occurred while publishing the message.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <param name="exception">Exception.</param>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="objectName">Object name.</param>
    /// <param name="message">Message content.</param>
    void LogMessagePublishingError<TMessage>(Exception exception, string instanceName, string objectName, TMessage message) where TMessage : IEventBusMessage;
}