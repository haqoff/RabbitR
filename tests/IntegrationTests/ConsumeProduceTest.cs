using IntegrationTests.Helpers;
using IntegrationTests.Helpers.Handlers;
using IntegrationTests.Helpers.Messages;
using Microsoft.Extensions.DependencyInjection;
using Rabbiter.Consumers.Handlers;

namespace IntegrationTests;

public class ConsumeProduceTest
{
    [Fact]
    public async Task Test_ProduceToExchange_ThenConsumeFromExchange_SingleMessage()
    {
        const string testExchangeName = "test-exchange";

        var handler = new TestMessageHandler<TextWithIntegerMessage>()
            .StopHandlingAtCall(1);

        await using var context = TestHelper.CreateContext(
            serviceCollectionBuilder: c => { c.AddScoped<IEventBusMessageHandler<TextWithIntegerMessage>>(_ => handler); },
            instanceCount: 1,
            builderAction: (containers, b) =>
            {
                var container = containers[0];

                b.AddInstance(c => TestHelper.BindConfig(c, container), instanceBuilder =>
                {
                    instanceBuilder.SetupConsumer(consumerConfig =>
                    {
                        consumerConfig.ConsumerGroup = "Test";
                        consumerConfig.MaxDegreeOfParallelism = 1;
                    }, consumerBuilder => { consumerBuilder.SubscribeExchange<TextWithIntegerMessage>(testExchangeName); });

                    instanceBuilder.SetupProducer(producerConfig =>
                    {
                        producerConfig.AttemptDelay = TimeSpan.FromSeconds(1);
                        producerConfig.MaxPoolLength = 1;
                        producerConfig.MaxRetryCount = 1;
                    });
                });
            });

        await context.StartAllRabbitMqAsync();
        await context.RunHostedServicesAsync();

        var publishedMessage = new TextWithIntegerMessage("some text", 1);
        await context.GetPublisher().PublishToExchangeAsync(testExchangeName, publishedMessage);

        await handler.WaitUntilDoneAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(1, handler.ReceivedMessages.Count);
        Assert.Equal(publishedMessage, handler.ReceivedMessages.First());
    }

    [Fact]
    public async Task Test_ProduceToExchange_ThenConsumeFromExchange_SingleMessage_DeadLetter()
    {
        const string testExchangeName = "test-exchange";

        var handler = new TestMessageHandler<TextWithIntegerMessage>()
            .StopHandlingAtCall(2)
            .ThrowErrorAtCall(1);

        await using var context = TestHelper.CreateContext(
            serviceCollectionBuilder: c => { c.AddSingleton<IEventBusMessageHandler<TextWithIntegerMessage>>(_ => handler); },
            instanceCount: 1,
            builderAction: (containers, b) =>
            {
                var container = containers[0];

                b.AddInstance(c => TestHelper.BindConfig(c, container), instanceBuilder =>
                {
                    instanceBuilder.SetupConsumer(consumerConfig =>
                    {
                        consumerConfig.ConsumerGroup = "Test";
                        consumerConfig.MaxDegreeOfParallelism = 2;
                    }, consumerBuilder =>
                    {
                        consumerBuilder.SubscribeExchange<TextWithIntegerMessage>(testExchangeName, subscriptionBuilder =>
                        {
                            subscriptionBuilder.SetMaxDegreeOfParallelism(2);
                            subscriptionBuilder.UseDeadLetter(deadLetterBuilder =>
                            {
                                deadLetterBuilder.SetDelay(TimeSpan.FromSeconds(1));
                                deadLetterBuilder.SetMaxRetryCount(2);
                            });
                        });
                    });

                    instanceBuilder.SetupProducer(producerConfig =>
                    {
                        producerConfig.AttemptDelay = TimeSpan.FromSeconds(1);
                        producerConfig.MaxPoolLength = 1;
                        producerConfig.MaxRetryCount = 1;
                    });
                });
            });

        await context.StartAllRabbitMqAsync();
        await context.RunHostedServicesAsync();

        var publishedMessage = new TextWithIntegerMessage("some text", 1);
        await context.GetPublisher().PublishToExchangeAsync(testExchangeName, publishedMessage);

        await handler.WaitUntilDoneAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(2, handler.ReceivedMessages.Count);
        foreach (var receivedMessage in handler.ReceivedMessages)
        {
            Assert.Equal(publishedMessage, receivedMessage);
        }
    }

    [Fact]
    public async Task Test_ProduceToExchange_ThenConsumeFromExchange_1000Messages()
    {
        const string testExchangeName = "test-exchange";

        var handler = new TestMessageHandler<TextWithIntegerMessage>()
            .StopHandlingAtCall(1000);

        await using var context = TestHelper.CreateContext(
            serviceCollectionBuilder: c => { c.AddScoped<IEventBusMessageHandler<TextWithIntegerMessage>>(_ => handler); },
            instanceCount: 1,
            builderAction: (containers, b) =>
            {
                var container = containers[0];

                b.AddInstance(c => TestHelper.BindConfig(c, container), instanceBuilder =>
                {
                    instanceBuilder.SetupConsumer(consumerConfig =>
                    {
                        consumerConfig.ConsumerGroup = "Test";
                        consumerConfig.MaxDegreeOfParallelism = 5;
                    }, consumerBuilder => { consumerBuilder.SubscribeExchange<TextWithIntegerMessage>(testExchangeName, exchangeBuilder => { exchangeBuilder.SetMaxDegreeOfParallelism(5); }); });

                    instanceBuilder.SetupProducer(producerConfig =>
                    {
                        producerConfig.AttemptDelay = TimeSpan.FromSeconds(1);
                        producerConfig.MaxPoolLength = 10;
                        producerConfig.MaxRetryCount = 1;
                    });
                });
            });

        await context.StartAllRabbitMqAsync();
        await context.RunHostedServicesAsync();

        var publishingMessages = Enumerable
            .Range(1, 1000)
            .Select(i => new TextWithIntegerMessage($"text {i}", i))
            .ToArray();

        foreach (var message in publishingMessages)
        {
            await context.GetPublisher().PublishToExchangeAsync(testExchangeName, message);
        }

        await handler.WaitUntilDoneAsync(TimeSpan.FromSeconds(30));

        // wait a bit, suddenly there will be extra messages.
        await Task.Delay(500);

        Assert.Equal(1000, handler.ReceivedMessages.Count);
        foreach (var receivedMessage in handler.ReceivedMessages)
        {
            Assert.Contains(receivedMessage, publishingMessages);
        }
    }
}