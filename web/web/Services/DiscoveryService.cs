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
        private SemaphoreSlim _semaphore;

        public DiscoveryService(LoggingService logger, SettingsService settings)
        {
            _logger = logger;
            _settings = settings;

            var primary = new IPEndPoint(_settings.Subnet.GetPrimaryDnsServer(), _settings.Subnet.PrimaryDnsPort);
            var secondary = new IPEndPoint(_settings.Subnet.GetSecondaryDnsServer(), _settings.Subnet.SecondaryDnsPort);

            _dnsLookupClient = new LookupClient(primary, secondary);
            _semaphore = new(1, 1);
        }

        public async Task<SubnetContainer> ExecuteDiscoveryTasksAsync(SubnetContainer subnet)
        {
            await _semaphore.WaitAsync();
            _logger.LogInfo("Discovery task initiated.", _logSource);
            List<Task<IpMonitoringTarget>> tasks = new();

            foreach (var ip in subnet.Children)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                    {
                        return DiscoveryTask(ip);
                    }));
            }

            _logger.LogInfo("Discovery tasks created awaiting results.", _logSource);
            await Task.WhenAll(tasks);

            subnet.Children = tasks.Select(x => x.Result).ToList();

            _logger.LogInfo("Discovery tasks completed returning results.", _logSource);
            _semaphore?.Dispose();
            return subnet;
        }

        public IpMonitoringTarget DiscoveryTask(IpMonitoringTarget ip)
        {
            using var ping = new Ping();
            var ipAddress = new IPAddress(ip.Address);

            if (ping.Send(ipAddress).Status == IPStatus.Success)
            {
                _logger.LogInfo($"{(IpMonitoringTarget.ConvertToString(ip.Address))} responded to ping", _logSource);
                ip.IsMonitoredIcmp = true;

                try
                {
                    ip.Hostname = _dnsLookupClient.GetHostName(ipAddress) ?? string.Empty;
                    _logger.LogInfo($"{IpMonitoringTarget.ConvertToString(ip.Address)} resolved to {ip.Hostname}", _logSource);
                }
                catch { }
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
