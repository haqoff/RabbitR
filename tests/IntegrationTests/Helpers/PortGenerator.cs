using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace IntegrationTests.Helpers;

public class PortGenerator
{
    private readonly HashSet<int> _busyPorts;
    private readonly object _lock = new();
    private int _lastPort;

    public PortGenerator()
    {
        _busyPorts = GetBusyPorts();
        _lastPort = 1413;
    }

    public int GenerateNextPort()
    {
        lock (_lock)
        {
            while (_lastPort < 65535)
            {
                _lastPort++;
                if (_busyPorts.Contains(_lastPort))
                {
                    continue;
                }

                try
                {
                    var ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
                    var tcpListener = new TcpListener(ipAddress, _lastPort);
                    tcpListener.Start();
                    tcpListener.Stop();
                    return _lastPort;
                }
                catch (SocketException)
                {
                    // continue scan
                }
            }

            throw new InvalidOperationException("No free ports available!");
        }
    }

    private static HashSet<int> GetBusyPorts()
    {
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] activeTcpConnections = ipGlobalProperties.GetActiveTcpConnections();
        return new HashSet<int>(activeTcpConnections.Select(t => t.LocalEndPoint.Port));
    }
}