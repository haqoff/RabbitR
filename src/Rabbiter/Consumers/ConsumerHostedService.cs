using Microsoft.Extensions.Hosting;
using Rabbiter.Builders;

namespace Rabbiter.Consumers;

/// <summary>
/// Represents a hosted service that starts consuming messages.
/// </summary>
internal class ConsumerHostedService : IHostedService
{
    private readonly IConsumer _consumer;
    private readonly RabbiterBuildResult _buildResult;
    private CancellationTokenSource _cts = new();
    private Task? _executingTask;
    private readonly TaskCompletionSource _creatingCompletionSource;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConsumerHostedService"/>.
    /// </summary>
    public ConsumerHostedService(IConsumer consumer, RabbiterBuildResult buildResult)
    {
        _consumer = consumer;
        _buildResult = buildResult;
        _creatingCompletionSource = new TaskCompletionSource();
    }

    /// <summary>
    /// A task that will complete when all consumers have been configured and created.
    /// </summary>
    public Task CreationCompletion => _creatingCompletionSource.Task;

    /// <summary>
    /// Starts processing.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = Task.Run(HandleCore, _cts.Token);

        return Task.CompletedTask;
    }


    /// <summary>
    /// Indicates that it is necessary to stop consuming messages.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _cts.Cancel();
        }
        finally
        {
            if (_executingTask is not null)
            {
                await _executingTask;
            }
        }
    }

    private void HandleCore()
    {
        foreach (var instance in _buildResult.Instances)
        {
            if (instance.Consumer is null)
            {
                continue;
            }

            _consumer.Start(instance, _cts.Token);
        }

        _creatingCompletionSource.SetResult();
    }
}