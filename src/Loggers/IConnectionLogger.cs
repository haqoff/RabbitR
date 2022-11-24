using RabbitMQ.Client.Exceptions;
using RabbitR.Connections;

namespace RabbitR.Loggers;

/// <summary>
/// Provides methods for logging a connection state.
/// </summary>
internal interface IConnectionLogger
{
    /// <summary>
    /// Indicates that a connection will be attempted.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    void LogBeforeStart(string instanceName, ConnectionConfig config);

    /// <summary>
    /// Indicates that there was an error during the connection, but there were attempts to reconnect.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    /// <param name="reconnectNumber">How many attempts were made to reconnect.</param>
    /// <param name="exception">Connection exception.</param>
    void LogFailRetryAgain(string instanceName, ConnectionConfig config, int reconnectNumber, BrokerUnreachableException exception);

    /// <summary>
    /// Indicates that an error occurred during the connection. No more attempts to reconnect.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    /// <param name="exception">Connection exception.</param>
    void LogFailNoRetriesLeft(string instanceName, ConnectionConfig config, BrokerUnreachableException exception);

    /// <summary>
    /// Indicates that the connection was successfully established.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    void LogConnectionSuccess(string instanceName, ConnectionConfig config);
}