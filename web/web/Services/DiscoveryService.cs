using DashLib;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using DashLib.Models.Network;
using DashLib.Models;

namespace web.Services
{
    public class DiscoveryService(LoggingService logger)
    {
        private readonly LoggingService _logger = logger;
        private static readonly LogEntry.LogSource _logSource = LogEntry.LogSource.DiscoveryService;

        public async Task<Subnet> ExecuteDiscoveryTasksAsync(Subnet subnet)
        {
            _logger.LogInfo("Discovery task initiated.", _logSource);
            List<Task<IP>> tasks = new();

            foreach (var ip in subnet.List)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                    {
                        return DiscoveryTask(ip);
                    }));
            }

            _logger.LogInfo("Discovery tasks created awaiting results.", _logSource);
            await Task.WhenAll(tasks);

            subnet.List = tasks.Select(x => x.Result).ToList();

            _logger.LogInfo("Discovery tasks completed returning results.", _logSource);
            return subnet;
        }

        public IP DiscoveryTask(IP ip)
        {
            using var ping = new Ping();

            _logger.LogInfo($"Scanning {IP.ConvertToString(ip.Address)}", _logSource);

            var ipAddress = new IPAddress(ip.Address);

            if (ping.Send(ipAddress).Status == IPStatus.Success)
            {
                _logger.LogInfo($"{(IP.ConvertToString(ip.Address))} responded to ping", _logSource);
                ip.IsMonitoredICMP = true;

                try
                {
                    ip.Hostname = Dns.GetHostEntry(ipAddress).HostName;
                    _logger.LogInfo($"{IP.ConvertToString(ip.Address)} resolved to {ip.Hostname}", _logSource);
                }
                catch (SocketException)
                {
                    ip.Hostname = ip.Hostname;
                }
            }

            return ip;
        }
    }
}
