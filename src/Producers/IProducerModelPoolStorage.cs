using RabbitMQ.Client;
using RabbitR.Utils;

namespace RabbitR.Producers;

/// <summary>
/// Provides a method for getting the pool of channels for an instance producer.
/// </summary>
internal interface IProducerModelPoolStorage : IDisposable
{
    /// <summary>
    /// Gets the channel pool for the specified instance named <paramref name="instanceName"/>.
    /// </summary>
    /// <exception cref="KeyNotFoundException">The exception that is thrown when no pool is found for the instance.</exception>
    LimitedPool<IModel> Get(string instanceName);
}