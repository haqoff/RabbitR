using Rabbiter.Builders.Instances.Consumers;
using Rabbiter.Builders.Instances.Operations;
using Rabbiter.Builders.Instances.Operations.Results;
using Rabbiter.Builders.Instances.Producers;
using Rabbiter.Connections;
using Rabbiter.Consumers.Configurations;
using Rabbiter.Producers;

namespace Rabbiter.Builders.Instances;

/// <summary>
/// Represents an instance builder that allows you to configure consumers and producers.
/// </summary>
public class InstanceBuilder
{
    private readonly string _name;
    private readonly ConnectionConfig _connectionConfig;
    private ConsumerBuildResult? _consumer;
    private ProducerBuildResult? _producer;
    private InitOperationContainerBuildResult? _initOperationContainer;

    /// <summary>
    /// Initializes a new instance of the class <see cref="InstanceBuilder"/>.
    /// </summary>
    internal InstanceBuilder(string name, ConnectionConfig connectionConfig)
    {
        _name = name;
        _connectionConfig = connectionConfig;
    }

    /// <summary>
    /// Specifies that this instance will consume messages.
    /// </summary>
    /// <param name="bindConfig">Action that configures the consumer config.</param>
    /// <param name="buildAction">Action that performs the consumer configuration.</param>
    public InstanceBuilder SetupConsumer(Action<ConsumerConfig> bindConfig, Action<ConsumerBuilder> buildAction)
    {
        var consumerConfig = new ConsumerConfig();
        bindConfig(consumerConfig);

        var builder = new ConsumerBuilder(consumerConfig);
        buildAction(builder);

        _consumer = builder.Build();
        return this;
    }

    /// <summary>
    /// Indicates that this instance can publish messages.
    /// </summary>
    /// <param name="bindConfig">Action that configures the producer config.</param>
    public InstanceBuilder SetupProducer(Action<ProducerConfig> bindConfig)
    {
        var config = new ProducerConfig();
        bindConfig(config);

        _producer = new ProducerBuildResult(config);
        return this;
    }

    /// <summary>
    /// Configures the operations that must be performed to initialize Rabbit MQ. For example, creating queues, exchange or binding them.
    /// Operations are performed immediately after a successful connection is established.
    /// </summary>
    /// <param name="builderAction">Action that configures operations.</param>
    public InstanceBuilder SetupInitOperations(Action<InitOperationBuilder> builderAction)
    {
        var builder = new InitOperationBuilder();
        builderAction(builder);

        _initOperationContainer = builder.Build();
        return this;
    }

    /// <summary>
    /// Builds the result.
    /// </summary>
    internal InstanceBuildResult Build()
    {
        return new InstanceBuildResult(_name, _connectionConfig, _consumer, _producer, _initOperationContainer);
    }
}