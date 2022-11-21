using Rabbiter.Messages;

namespace Rabbiter.Consumers.Handlers;

/// <summary>
/// Represents a message handler.
/// </summary>
public interface IEventBusMessageHandler<in T> where T : IEventBusMessage
{
    /// <summary>
    /// Performs processing on the specified message.
    /// </summary>
    public Task HandleAsync(T message, CancellationToken cancellationToken);
}