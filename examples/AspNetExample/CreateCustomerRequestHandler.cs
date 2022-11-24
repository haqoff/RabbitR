using Rabbiter.Consumers.Handlers;
using Rabbiter.Producers;

namespace AspNetExample;

public class CreateCustomerRequestHandler : IEventBusMessageHandler<CreateCustomerRequestMessage>
{
    private readonly IEventBusPublisher _publisher;
    private readonly Random _random;

    public CreateCustomerRequestHandler(IEventBusPublisher publisher)
    {
        _publisher = publisher;
        _random = new Random();
    }

    public Task HandleAsync(CreateCustomerRequestMessage message, CancellationToken cancellationToken)
    {
        // some kind of logic for adding a customer to the database.
        var response = new CustomerCreatedMessage(_random.Next(1, 1000), message.FullName, DateTime.Now);

        // publish a new message to the default instance with exchange name customer_created_exchange.
        _publisher.PublishToExchangeAsync(CustomerCreatedMessage.ExchangeName, response);

        // for example, you can publish a message to another connection instance, and also specify a routing key so that the receiving party can filter messages.
        //_publisher.PublishToExchangeAsync("OtherInstance", CustomerCreatedMessage.ExchangeName, response, "routingKey");

        return Task.CompletedTask;
    }
}