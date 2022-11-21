using System.ComponentModel.DataAnnotations;
using Rabbiter.Connections;
using Rabbiter.Utils;

namespace Rabbiter.Builders.Results;

/// <summary>
/// Represents the result of building instance.
/// </summary>
internal class InstanceBuildResult
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="InstanceBuildResult"/>.
    /// </summary>
    internal InstanceBuildResult(string name, ConnectionConfig connectionConfig, ConsumerBuildResult? consumer, ProducerBuildResult? producer)
    {
        RabbiterValidator.ThrowIfNotValid(connectionConfig);
        if (consumer is null && producer is null)
        {
            throw new ValidationException($"When creating an instance with name \"{name}\" for {connectionConfig}, neither a consumer nor a producer was specified.");
        }

        Name = name;
        ConnectionConfig = connectionConfig;
        Consumer = consumer;
        Producer = producer;
    }

    /// <summary>
    /// Instance name.
    /// </summary>
    internal string Name { get; }

    /// <summary>
    /// Connection config of instance.
    /// </summary>
    internal ConnectionConfig ConnectionConfig { get; }

    /// <summary>
    /// Consumer settings.
    /// </summary>
    internal ConsumerBuildResult? Consumer { get; }

    /// <summary>
    /// Producer settings.
    /// </summary>
    internal ProducerBuildResult? Producer { get; }
}