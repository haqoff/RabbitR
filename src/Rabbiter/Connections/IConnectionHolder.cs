using Rabbiter.Builders.Results;
using RabbitMQ.Client;

namespace Rabbiter.Connections;

/// <summary>
/// Provides a method for getting or creating a connection, if not already created.
/// </summary>
internal interface IConnectionHolder : IDisposable
{
    /// <summary>
    /// Creates a new connection if none has been created for the specified instance name.
    /// Otherwise, returns the existing one.
    /// </summary>
    IConnection GetOrCreateConnection(InstanceBuildResult instance);
}