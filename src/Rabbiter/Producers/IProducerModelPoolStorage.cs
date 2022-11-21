using Rabbiter.Utils;
using RabbitMQ.Client;

namespace Rabbiter.Producers;

/// <summary>
/// Provides a method for getting the pool of channels for an instance producer.
/// </summary>
internal interface IProducerModelPoolStorage : IDisposable
{
    /// <summary>
    /// Gets the channel pool for the specified instance named <paramref name="instanceName"/>.
    /// </summary>
    LimitedPool<IModel> Get(string instanceName);
}