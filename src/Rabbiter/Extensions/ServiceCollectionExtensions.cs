using Microsoft.Extensions.DependencyInjection;
using Rabbiter.Builders;
using Rabbiter.Connections;
using Rabbiter.Consumers;
using Rabbiter.Consumers.Configurations;
using Rabbiter.Consumers.Handlers;
using Rabbiter.Loggers;
using Rabbiter.Producers;

namespace Rabbiter.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a Rabbiter to the collection of services.
    /// </summary>
    /// <param name="collection">Collection of services.</param>
    /// <param name="buildAction">Action to configure connection instances.</param>
    /// <returns>Collection of services.</returns>
    public static IServiceCollection AddRabbiter(this IServiceCollection collection, Action<RabbiterBuilder> buildAction)
    {
        var builder = new RabbiterBuilder();
        buildAction(builder);

        var result = builder.Build();

        // connection
        collection.AddSingleton<IConnectionLogger, ConnectionLogger>();
        collection.AddSingleton<IConnectionHolder, ConnectionHolder>();

        // consumer
        collection.AddSingleton<IConsumerLogger, ConsumerLogger>();
        collection.AddSingleton<IConsumerObjectConfigurator, ConsumerObjectСonfigurator>();
        collection.AddSingleton<IConsumer, Consumer>();
        collection.AddSingleton(p => new ConsumerHostedService(p.GetRequiredService<IConsumer>(), result));
        collection.AddHostedService(p => p.GetRequiredService<ConsumerHostedService>());
        collection.AddConsumerHandlers(result);

        // producer
        collection.AddSingleton<IProducerLogger, ProducerLogger>();
        collection.AddSingleton<IProducerModelPoolStorage>(p => new ProducerModelPoolStorage(p.GetRequiredService<IConnectionHolder>(), result.Instances));
        collection.AddSingleton<IEventBusPublisher>(p => new EventBusPublisher(
            p.GetRequiredService<IProducerModelPoolStorage>(),
            p.GetRequiredService<ConsumerHostedService>().CreationCompletion,
            p.GetRequiredService<IProducerLogger>())
        );

        return collection;
    }

    private static void AddConsumerHandlers(this IServiceCollection collection, RabbiterBuildResult result)
    {
        var handlerInterfaceType = typeof(IEventBusMessageHandler<>);
        foreach (var instance in result.Instances)
        {
            if (instance.Consumer is null)
            {
                continue;
            }

            foreach (var subscription in instance.Consumer.Items)
            {
                var serviceType = handlerInterfaceType.MakeGenericType(subscription.MessageType);
                collection.AddScoped(serviceType, subscription.HandlerType);
            }
        }
    }
}