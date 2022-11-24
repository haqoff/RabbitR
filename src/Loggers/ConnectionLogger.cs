using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using RabbitR.Connections;

namespace RabbitR.Loggers;

/// <summary>
/// Represents a mechanism for connection logging.
/// </summary>
internal class ConnectionLogger : IConnectionLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="ConnectionLogger"/>.
    /// </summary>
    public ConnectionLogger(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger(LoggerNamespace.Connection);
    }

    /// <summary>
    /// Indicates that a connection will be attempted.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    public void LogBeforeStart(string instanceName, ConnectionConfig config)
    {
        _logger.LogInformation("Start trying to connect to an instance with name \"{instanceName}\" and config {config}.", instanceName, config.ToString());
    }

    /// <summary>
    /// Indicates that there was an error during the connection, but there were attempts to reconnect.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    /// <param name="reconnectNumber">How many attempts were made to reconnect.</param>
    /// <param name="exception">Connection exception.</param>
    public void LogFailRetryAgain(string instanceName, ConnectionConfig config, int reconnectNumber, BrokerUnreachableException exception)
    {
        _logger.LogWarning(exception, "Failed to connect to instance named \"{instanceName}\". {attemptLeft} more attempt left. Try to connect again in {timeout} seconds.", instanceName,
            config.MaxReconnectCount - reconnectNumber, config.RetryTimeout.TotalSeconds);
    }

    /// <summary>
    /// Indicates that an error occurred during the connection. No more attempts to reconnect.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    /// <param name="exception">Connection exception.</param>
    public void LogFailNoRetriesLeft(string instanceName, ConnectionConfig config, BrokerUnreachableException exception)
    {
        _logger.LogCritical(exception, "Failed to connect to instance named \"{instanceName}\".", instanceName);
    }

    /// <summary>
    /// Indicates that the connection was successfully established.
    /// </summary>
    /// <param name="instanceName">Instance name.</param>
    /// <param name="config">Connection config.</param>
    public void LogConnectionSuccess(string instanceName, ConnectionConfig config)
    {
        _logger.LogInformation("Successfully connected to instance named \"{instanceName}\" with config {config}.", instanceName, config.ToString());
    }
}