using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rabbiter.Builders;
using Rabbiter.Connections;
using Rabbiter.Extensions;

namespace IntegrationTests.Helpers;

internal static class TestHelper
{
    public static readonly Lazy<PortGenerator> PortGeneratorLazy = new(() => new PortGenerator(), true);

    public static TestcontainersContainer CreateDockerContainer(int connectionPort)
    {
        return new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("library/rabbitmq")
            .WithEnvironment("hostname", "my-rabbit")
            .WithPortBinding(connectionPort.ToString(), "5672")
            .WithName($"RABBIT-MQ-{Guid.NewGuid():N}")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
            .WithResourceReaperSessionId(Guid.NewGuid())
            .Build();
    }

    public static TestContext CreateContext(
        int instanceCount = 1,
        Action<IServiceCollection>? serviceCollectionBuilder = null,
        Action<IReadOnlyList<TestContainer>, RabbiterBuilder>? builderAction = null)
    {
        if (instanceCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(instanceCount));
        }

        var containers = new TestContainer[instanceCount];

        for (var i = 0; i < instanceCount; i++)
        {
            var connectionPort = PortGeneratorLazy.Value.GenerateNextPort();
            var docker = CreateDockerContainer(connectionPort);
            containers[i] = new TestContainer(docker, connectionPort);
        }

        var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Rabbiter", LogLevel.Debug);
            }
        );

        var collection = new ServiceCollection();
        collection.AddRabbiter(builderAction is not null ? b => builderAction(containers, b) : _ => { });
        collection.AddSingleton(loggerFactory);
        serviceCollectionBuilder?.Invoke(collection);
        var provider = collection.BuildServiceProvider();

        return new TestContext(containers, provider);
    }

    public static void BindConfig(ConnectionConfig config, TestContainer container)
    {
        config.HostName = "localhost";
        config.Port = container.ConnectionPort;
        config.UserName = "guest";
        config.Password = "guest";
        config.RetryTimeout = TimeSpan.FromSeconds(1);
        config.MaxReconnectCount = 1;
    }
}