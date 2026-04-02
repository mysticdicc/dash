using DashLib.DankAPI;
using DashLib.Interfaces.Monitoring;
using DashLib.Models;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace web.Services
{
    public class MonitorService : BackgroundService
    {
        private Timer? _timer;
        private readonly LoggingService _logger;
        CancellationTokenSource _cancellationToken;
        IMonitorStateRepository _monitoringRepo;
        SettingsService _currentSettings;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.MonitoringService;
        public MonitorService(LoggingService logger, IMonitorStateRepository monitoringRepo, SettingsService settingsService)
        {
            _timer = null;
            _logger = logger;
            _cancellationToken = new();
            _currentSettings = settingsService;
            _monitoringRepo = monitoringRepo;

            _logger.LogInfo("Service started", _logSource);
        }

        private async Task RunServiceAsync(CancellationToken token)
        {
            try
            {
                _logger.LogInfo($"Entered execution action", _logSource);
                await Task.Delay(1500);

                DateTime submit = DateTime.UtcNow;

                _logger.LogInfo("Fetching monitored devices from database", _logSource);

                var ipList = await _monitoringRepo.GetAllMonitoredIpsAsync();
                int ipUpdate = 0;
                _logger.LogInfo($"{ipList.Count} IP targets monitored.", _logSource);

                var dnsList = await _monitoringRepo.GetAllMonitoredDnsAsync();
                int dnsUpdate = 0;
                _logger.LogInfo($"{dnsList.Count} DNS targets monitored.", _logSource);

                foreach (var ip in ipList)
                {
                    switch ((ip.IsMonitoredIcmp, ip.IsMonitoredTcp))
                    {
                        case (true, true):
                            ip.IcmpMonitorStates.Add(await ip.IcmpTestAsync());
                            ip.TcpMonitorStates.AddRange(await ip.TcpTestAsync());
                            ipUpdate++;
                            break;

                        case (true, false):
                            ip.IcmpMonitorStates.Add(await ip.IcmpTestAsync());
                            ipUpdate++;
                            break;

                        case (false, true):
                            ip.TcpMonitorStates.AddRange(await ip.TcpTestAsync());
                            ipUpdate++;
                            break;
                    }
                }

                _logger.LogInfo($"{ipUpdate} IP monitor tasks completed and will be submitted to database.", _logSource);

                if (ipUpdate > 0)
                {
                    try
                    {
                        await _monitoringRepo.AddMonitorStatesFromListIpAsync(ipList.ToList());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to save IP states: {ex.Message}", _logSource);
                    }
                }

                foreach (var dns in dnsList)
                {
                    switch ((dns.IsMonitoredIcmp, dns.IsMonitoredTcp))
                    {
                        case (true, true):
                            dns.IcmpMonitorStates.Add(await dns.IcmpTestAsync());
                            dns.TcpMonitorStates.AddRange(await dns.TcpTestAsync());
                            dnsUpdate++;
                            break;

                        case (true, false):
                            dns.IcmpMonitorStates.Add(await dns.IcmpTestAsync());
                            dnsUpdate++;
                            break;

                        case (false, true):
                            dns.TcpMonitorStates.AddRange(await dns.TcpTestAsync());
                            dnsUpdate++;
                            break;
                    }
                }

                _logger.LogInfo($"{dnsUpdate} DNS monitor tasks completed and will be submitted to database.", _logSource);

                if (dnsUpdate > 0)
                {
                    try
                    {
                        await _monitoringRepo.AddMonitorStatesFromListDnsAsync(dnsList.ToList());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to save DNS states: {ex.Message}", _logSource);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInfo("User intiated service end", _logSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, _logSource);

                await Task.Delay(15000, token);
            }
        }

        async public void Restart()
        {
            _logger.LogInfo("Service restart initiated", _logSource);
            _cancellationToken.Cancel();
            _cancellationToken = new();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Service action has intiated", _logSource   );

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunServiceAsync(_cancellationToken.Token);
                await Task.Delay((_currentSettings.Monitoring.PollingIntervalInSeconds * 1000), stoppingToken);
            }
        }

        override public void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
