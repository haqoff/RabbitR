using AspNetExample;
using Rabbiter.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRabbiter(rabbiter =>
{
    // add a new instance.
    // this way you can support consuming/publishing from different rabbit mq server.
    // also in this method you can specify the name of the instance to distinguish between them when logging and sending messages,
    // if not specified, the default name "Default" will be used.
    rabbiter.AddInstance(
        connectionConfig => builder.Configuration.GetSection("RabbitMQ:Connection").Bind(connectionConfig),
        instance =>
        {
            // specify that from this instance we will consume messages
            instance.SetupConsumer(
                consumerConfig => builder.Configuration.GetSection("RabbitMQ:Consumer").Bind(consumerConfig),
                consumer =>
                {
                    // specify the name of the exchange, the type of message, the type of handler, and also configure if necessary
                    // the library itself will create the specified exchange, as well as the associated queue for consuming.
                    consumer.SubscribeExchange<CreateCustomerRequestMessage, CreateCustomerRequestHandler>("create_customer_request_exchange", exchange =>
                    {
                        exchange.SetMaxDegreeOfParallelism(2);
                        exchange.UseDeadLetter(deadLetter =>
                        {
                            deadLetter.SetDelay(TimeSpan.FromHours(1));
                            deadLetter.SetMaxRetryCount(3);
                        });
                    });
                });

            // indicate that we will also publish to this instance.
            instance.SetupProducer(producerConfig => builder.Configuration.GetSection("RabbitMQ:Producer").Bind(producerConfig));

            // if you need to create queues, exchanges, perform a binding, do it here.
            instance.SetupInitOperations(init =>
            {
                // when publishing messages, the exchange or queue is not created by itself, as this would be costly per publish call.
                // therefore, if we publish something, we need to make sure that the exchange is created.
                // you can do this manually on the Rabbit MQ server or here at the initialization stage.

                // create an exchange where we will publish a message when customer created.
                init.DeclareExchange(o => o.SetName(CustomerCreatedMessage.ExchangeName));
            });
        });

    // rabbiter.AddInstance("OtherInstance", ...)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
