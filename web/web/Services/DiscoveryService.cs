using DashLib;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using DashLib.Models.Network;
using DashLib.Models;
using DnsClient;

namespace web.Services
{
    public class DiscoveryService
    {
        private readonly LoggingService _logger;
        private readonly SettingsService _settings;
        private readonly LogEntry.LogSource _logSource = LogEntry.LogSource.DiscoveryService;
        private LookupClient _dnsLookupClient;

        public DiscoveryService(LoggingService logger, SettingsService settings)
        {
            _logger = logger;
            _settings = settings;

            var primary = new IPEndPoint(_settings.Subnet.GetPrimaryDnsServer(), _settings.Subnet.PrimaryDnsPort);
            var secondary = new IPEndPoint(_settings.Subnet.GetSecondaryDnsServer(), _settings.Subnet.SecondaryDnsPort);

            _dnsLookupClient = new LookupClient(primary, secondary);
        }

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
            var ipAddress = new IPAddress(ip.Address);

            if (ping.Send(ipAddress).Status == IPStatus.Success)
            {
                _logger.LogInfo($"{(IP.ConvertToString(ip.Address))} responded to ping", _logSource);
                ip.IsMonitoredICMP = true;

                try
                {
                    ip.Hostname = _dnsLookupClient.GetHostName(ipAddress) ?? string.Empty;
                    _logger.LogInfo($"{IP.ConvertToString(ip.Address)} resolved to {ip.Hostname}", _logSource);
                }
                catch (SocketException)
                {
                    ip.Hostname = ip.Hostname;
                }
            }

            return ip;
        }

        public async Task Restart()
        {
            _logger.LogInfo("Service restart initiated", _logSource);

            var primary = new IPEndPoint(_settings.Subnet.GetPrimaryDnsServer(), _settings.Subnet.PrimaryDnsPort);
            var secondary = new IPEndPoint(_settings.Subnet.GetSecondaryDnsServer(), _settings.Subnet.SecondaryDnsPort);

            _dnsLookupClient = new LookupClient(primary, secondary);
        }
    }
}
