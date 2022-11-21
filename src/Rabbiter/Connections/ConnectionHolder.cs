using System.Collections.Concurrent;
using Rabbiter.Builders.Results;
using Rabbiter.Loggers;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Rabbiter.Connections;

/// <summary>
/// Represents a connection store.
/// </summary>
internal class ConnectionHolder : IConnectionHolder
{
    private readonly IConnectionLogger _logger;
    private readonly ConcurrentDictionary<string, Lazy<IConnection>> _connectionDictionary;
    private volatile bool _disposed;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConnectionHolder"/>.
    /// </summary>
    public ConnectionHolder(IConnectionLogger logger)
    {
        _logger = logger;
        _connectionDictionary = new ConcurrentDictionary<string, Lazy<IConnection>>();
    }

    /// <summary>
    /// Creates a new connection if none has been created for the specified instance name.
    /// Otherwise, returns the existing one.
    /// </summary>
    public IConnection GetOrCreateConnection(InstanceBuildResult instance)
    {
        ThrowIfDisposed();

        var connectionLazy = _connectionDictionary.GetOrAdd(instance.Name, _ => new Lazy<IConnection>(() => CreateConnectionCore(instance), true));
        return connectionLazy.Value;
    }

    /// <summary>
    /// Disposes all connections.
    /// </summary>
    public void Dispose()
    {
        ThrowIfDisposed();
        _disposed = true;

        var items = _connectionDictionary.ToArray();
        foreach (var item in items)
        {
            var lazyConnection = item.Value;
            lazyConnection.Value.Dispose();
            _connectionDictionary.TryRemove(item.Key, out _);
        }
    }

    private IConnection CreateConnectionCore(InstanceBuildResult instance)
    {
        var connectionConfig = instance.ConnectionConfig;

        // Despite AutomaticRecoveryEnabled = true, this mechanism does not work on the first connection attempt, so we must ensure this manually.
        var reconnectNumber = 0;
        while (reconnectNumber <= connectionConfig.MaxReconnectCount)
        {
            ThrowIfDisposed();

            var factory = new ConnectionFactory()
            {
                HostName = connectionConfig.Host,
                Port = connectionConfig.Port,
                UserName = connectionConfig.UserName,
                Password = connectionConfig.Password,
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = connectionConfig.RetryTimeout,
                ConsumerDispatchConcurrency = instance.Consumer?.ConsumerConfig.MaxDegreeOfParallelism ?? 1
            };

            var lastRetry = reconnectNumber == connectionConfig.MaxReconnectCount;

            try
            {
                _logger.LogBeforeStart(instance.Name, connectionConfig);
                var connection = factory.CreateConnection();
                _logger.LogConnectionSuccess(instance.Name, connectionConfig);
                return connection;
            }
            catch (BrokerUnreachableException e)
            {
                if (lastRetry)
                {
                    _logger.LogFailNoRetriesLeft(instance.Name, connectionConfig, e);
                    throw;
                }

                _logger.LogFailRetryAgain(instance.Name, connectionConfig, reconnectNumber, e);
            }

            reconnectNumber++;
            Thread.Sleep(connectionConfig.RetryTimeout);
        }

        throw new InvalidOperationException("Should not be happen");
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ConnectionHolder));
        }
    }
}