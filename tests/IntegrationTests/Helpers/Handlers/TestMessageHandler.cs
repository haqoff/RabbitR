using System.Collections.Concurrent;
using RabbitR.Consumers.Handlers;
using RabbitR.Messages;

namespace IntegrationTests.Helpers.Handlers;

internal class TestMessageHandler<TMessage> : IEventBusMessageHandler<TMessage> where TMessage : IEventBusMessage
{
    private readonly List<int> _whenThrowErrorCallNumbers;
    private readonly TaskCompletionSource _handlingCompletionSource;
    private readonly ConcurrentBag<TMessage> _messages;
    private int _currentCall;
    private int _stopHandlingCallNumber = 1;

    public TestMessageHandler()
    {
        _whenThrowErrorCallNumbers = new List<int>();
        _handlingCompletionSource = new TaskCompletionSource();
        _messages = new ConcurrentBag<TMessage>();
    }

    public IReadOnlyCollection<TMessage> ReceivedMessages => _messages;

    public TestMessageHandler<TMessage> ThrowErrorAtCall(int callNumber)
    {
        _whenThrowErrorCallNumbers.Add(callNumber);
        return this;
    }

    public TestMessageHandler<TMessage> StopHandlingAtCall(int callNumber)
    {
        _stopHandlingCallNumber = callNumber;
        return this;
    }

    public Task WaitUntilDoneAsync(TimeSpan timeout)
    {
        return Task.WhenAny(_handlingCompletionSource.Task, Task.Delay(timeout));
    }

    public Task HandleAsync(TMessage message, CancellationToken cancellationToken)
    {
        _messages.Add(message);

        var currentCall = Interlocked.Increment(ref _currentCall);
        if (_whenThrowErrorCallNumbers.Any(n => n == currentCall))
        {
            throw new InvalidOperationException("some error during processing...");
        }

        if (currentCall == _stopHandlingCallNumber)
        {
            _handlingCompletionSource.TrySetResult();
        }

        return Task.CompletedTask;
    }
}