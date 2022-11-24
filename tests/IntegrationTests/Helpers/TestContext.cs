using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitR.Producers;

namespace IntegrationTests.Helpers;

internal class TestContext : IAsyncDisposable
{
    public IReadOnlyList<TestContainer> Containers { get; }
    public ServiceProvider Provider { get; }

    public TestContext(IReadOnlyList<TestContainer> containers, ServiceProvider provider)
    {
        Containers = containers;
        Provider = provider;
    }

    public async Task StartAllRabbitMqAsync()
    {
        foreach (var container in Containers)
        {
            await container.Docker.StartAsync();
        }
    }

    public async Task StopAllRabbitMqAsync()
    {
        foreach (var container in Containers)
        {
            await container.Docker.StopAsync();
        }
    }

    public async Task RunHostedServicesAsync()
    {
        var services = Provider.GetServices<IHostedService>();
        foreach (var service in services)
        {
            await service.StartAsync(CancellationToken.None);
        }
    }

    public IEventBusPublisher GetPublisher()
    {
        return Provider.GetRequiredService<IEventBusPublisher>();
    }

    public async ValueTask DisposeAsync()
    {
        var services = Provider.GetServices<IHostedService>();
        foreach (var service in services)
        {
            try
            {
                await service.StopAsync(CancellationToken.None);
            }
            catch
            {
                // does not care
            }
        }

        try
        {
            await Provider.DisposeAsync();
        }
        catch
        {
            // does not care
        }

        foreach (var container in Containers)
        {
            try
            {
                await container.Docker.DisposeAsync();
            }
            catch
            {
                // does not care
            }
        }
    }
}