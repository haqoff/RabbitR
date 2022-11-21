using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rabbiter.Messages;

namespace Rabbiter.Loggers;

/// <summary>
/// Represents a mechanism for logging producers.
/// </summary>
internal class ProducerLogger : IProducerLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ProducerLogger"/>.
    /// </summary>
    public ProducerLogger(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger(LoggerNamespace.Producer);
    }

    /// <summary>
    /// Indicates that the publication of the message was successful.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="objectName">Object name.</param>
    /// <param name="message">Message content.</param>
    public void LogMessagePublishingSuccess<TMessage>(string instanceName, string objectName, TMessage message) where TMessage : IEventBusMessage
    {
        // TODO: enrich log by content instead write it to message directly
        var content = JsonSerializer.Serialize(message);
        _logger.LogInformation("Message {messageName} with content {content} was successfully published to {objectName} (instance: {instanceName}).", typeof(TMessage).Name, content, objectName,
            instanceName);
    }

    /// <summary>
    /// Indicates that an error occurred while publishing the message.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    /// <param name="exception">Exception.</param>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="objectName">Object name.</param>
    /// <param name="message">Message content.</param>
    public void LogMessagePublishingError<TMessage>(Exception exception, string instanceName, string objectName, TMessage message) where TMessage : IEventBusMessage
    {
        // TODO: enrich log by content instead write it to message directly
        var content = JsonSerializer.Serialize(message);
        _logger.LogError(exception, "An error occurred while posting message {messageName} with content {content} to object {objectName} (instance: {instanceName}).", typeof(TMessage).Name, content,
            objectName,
            instanceName);
    }
}