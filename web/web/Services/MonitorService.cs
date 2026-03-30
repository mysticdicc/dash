using DashLib.DankAPI;
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
        private readonly HttpClient _httpClient;
        private readonly LoggingService _logger;
        CancellationTokenSource _cancellationToken;
        MonitorStateAPI _monitoringApi;
        SettingsService _currentSettings;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.MonitoringService;
        public MonitorService(LoggingService logger, HttpClient httpClient, MonitorStateAPI monitoringApi, SettingsService settingsService)
        {
            _httpClient = httpClient;
            _timer = null;
            _logger = logger;
            _cancellationToken = new();
            _currentSettings = settingsService;
            _monitoringApi = monitoringApi;

            _logger.LogInfo("Service started", _logSource);
        }

        private async Task RunServiceAsync(CancellationToken token)
        {
            try
            {
                _logger.LogInfo($"Entered execution action", _logSource);
                await Task.Delay(1500);

                DateTime submit = DateTime.UtcNow;

                List<IP>? ips = [];

                _logger.LogInfo("Fetching monitored devices from API endpoint", _logSource);
                ips = await _monitoringApi.GetMonitoredIpsAsync();

                if (null != ips)
                {
                    _logger.LogInfo($"Fetched {ips.Count()} from API", _logSource);

                    foreach (IP ip in ips)
                    {
                        MonitorState currentMonitorState = new()
                        {
                            SubmitTime = submit,
                            IP_ID = ip.ID
                        };

                        ip.MonitorStateList = [];

                        switch ((ip.IsMonitoredICMP, ip.IsMonitoredTCP))
                        {
                            case (true, true):
                                currentMonitorState.PortState = TcpTest(ip);
                                currentMonitorState.PingState = IcmpTest(ip);
                                break;
                            case (true, false):
                                currentMonitorState.PingState = IcmpTest(ip);
                                break;
                            case (false, true):
                                currentMonitorState.PortState = TcpTest(ip);
                                break;
                        }

                        ip.MonitorStateList.Add(currentMonitorState);

                    }

                    try
                    {
                        await _monitoringApi.PostNewDevicePollAsync(ips);
                        _logger.LogInfo("Ips submitted to api endpoint", _logSource);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, _logSource);
                    }
                }
                else
                {
                    _logger.LogError("Could not fetch IP list from api endpoint", _logSource);
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

        List<PortState> TcpTest(IP ip)
        {
            _logger.LogInfo($"TCP testing {IP.ConvertToString(ip.Address)}", _logSource);

            List<PortState> portStates = [];
            
            if (null != ip.PortsMonitored)
            {
                foreach (int port in ip.PortsMonitored)
                {
                    _logger.LogInfo($"Testing {port.ToString()} on {IP.ConvertToString(ip.Address)}", _logSource);

                    using var client = new TcpClient();
                    bool status = false;

                    try
                    {
                        var address = new IPAddress(ip.Address);
                        client.Connect(address, port);
                        status = true;
                    }
                    catch
                    {
                        status = false;
                    }

                    PortState state = new()
                    {
                        Port = port,
                        Status = status
                    };

                    portStates.Add(state);
                }
            }

            return portStates;
        }

        PingState IcmpTest(IP ip)
        {
            _logger.LogInfo($"Ping testing {IP.ConvertToString(ip.Address)}", _logSource);

            using Ping ping = new();

            PingState pingState = new()
            {
                Response = ping.Send(new IPAddress(ip.Address)).Status == IPStatus.Success
            };

            return pingState;
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
