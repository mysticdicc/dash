using danklibrary.DankAPI;
using danklibrary.Monitoring;
using danklibrary.Network;
using danklibrary.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        public int _delay;
        private ILogger<MonitorService> _logger;
        CancellationTokenSource _cancellationToken;
        MonitoringAPI _monitoringApi;
        AllSettings _currentSettings;

        public MonitorService(ILogger<MonitorService> logger, HttpClient httpClient, MonitoringAPI monitoringApi)
        {
            _httpClient = httpClient;
            _timer = null;
            _logger = logger;
            _cancellationToken = new();
            _monitoringApi = monitoringApi;
            try
            {
                _currentSettings = AllSettings.GetCurrentSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));
            }
            catch
            {
                AllSettings.CreateNewSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));
                _currentSettings = AllSettings.GetCurrentSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));
            }
            
            _delay = _currentSettings.MonitoringSettings.PollingIntervalInSeconds;
        }

        private async Task RunServiceAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _logger.LogInformation($"Entered execution action, task delay is {(_delay * 1000)}s");
                    await Task.Delay(1500);

                    DateTime submit = DateTime.UtcNow;

                    //fetch monitored devices
                    var handler = new HttpClientHandler();
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback =
                        (httpRequestMessage, cert, cetChain, policyErrors) =>
                        {
                            return true;
                        };

                    List<IP>? ips = [];

                    _logger.LogInformation("Fetching monitored devices from API endpoint");
                    ips = await _monitoringApi.GetMonitoredIpsAsync();

                    if (null != ips)
                    {
                        _logger.LogInformation($"Fetched {ips.Count()} from API");

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

                            ip.MonitorStateList = ip.MonitorStateList.Append(currentMonitorState);

                        }

                        _logger.LogInformation(JsonConvert.SerializeObject(ips, Formatting.Indented));

                        try
                        {
                            await _monitoringApi.PostNewDevicePollAsync(ips);
                            _logger.LogInformation("Ips submitted to api endpoint");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                            _logger.LogError(ex.InnerException?.Message);
                            _logger.LogError(ex.StackTrace);
                        }
                    }
                    else
                    {
                        _logger.LogError("Could not fetch IP list from api endpoint");
                    }

                    await Task.Delay((_delay * 1000), token);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("User intiated service end");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("=== HTTP REQUEST EXCEPTION ===");
                _logger.LogError(ex.HttpRequestError.ToString());
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.Data.ToString());
                _logger.LogError(ex.HResult.ToString());
                _logger.LogError(ex.Source);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("=== EXCEPTION ===");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.Data.ToString());
                _logger.LogError(ex.HResult.ToString());
                _logger.LogError(ex.Source);

                await Task.Delay(15000, token);
            }
        }

        List<PortState> TcpTest(IP ip)
        {
            _logger.LogInformation($"TCP testing {IP.ConvertToString(ip.Address)}");

            List<PortState> portStates = [];
            
            if (null != ip.PortsMonitored)
            {
                foreach (int port in ip.PortsMonitored)
                {
                    _logger.LogInformation($"Testing {port.ToString()} on {IP.ConvertToString(ip.Address)}");

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
            _logger.LogInformation($"Ping testing {IP.ConvertToString(ip.Address)}");

            using Ping ping = new();

            PingState pingState = new()
            {
                Response = ping.Send(new IPAddress(ip.Address)).Status == IPStatus.Success
            };

            return pingState;
        }

        async public void Restart()
        {
            _logger.LogInformation("Monitoring service restart initiated");
            _currentSettings = AllSettings.GetCurrentSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));
            _delay = _currentSettings.MonitoringSettings.PollingIntervalInSeconds;
            _cancellationToken.Cancel();
            _cancellationToken = new();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service action has intiated");

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunServiceAsync(_cancellationToken.Token);
            }
        }

        override public void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
