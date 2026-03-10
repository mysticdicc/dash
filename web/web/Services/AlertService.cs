using DashComponents.Monitoring;
using DashLib.DankAPI;
using DashLib.API;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using DashLib.Interfaces;
using DashLib.Interfaces.Monitoring;
using System.Text;

namespace web.Services
{
    public class AlertService : BackgroundService, IDisposable
    {
        private Timer? _timer;
        private ILogger<MonitorService> _logger;
        CancellationTokenSource _cancellationToken;
        MonitoringAPI _monitoringApi;
        MailAPI _mailApi;
        DiscordService _discordService;
        TelegramService _telegramService;
        SettingsService _settings;
        private int _alertEvalTimeInMinutes;

        public AlertService(
            ILogger<MonitorService> logger, 
            MonitoringAPI monitoringApi, 
            MailAPI mailApi, 
            DiscordService discordAPI,
            SettingsService settingsService,
            TelegramService telegramService)
        {
            _logger = logger;
            _monitoringApi = monitoringApi;
            _mailApi = mailApi;
            _discordService = discordAPI;
            _telegramService = telegramService;
            _cancellationToken = new CancellationTokenSource(); 
            _settings = settingsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service action has intiated");

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunServiceAsync(_cancellationToken.Token);
            }
        }

        private async Task RunServiceAsync(CancellationToken token)
        {
            try
            {
                do
                {
                    if (_settings.Monitoring.AlertsEnabled)
                    {
                        await RunServiceAction(token);
                    }
                    else
                    {
                        _logger.LogInformation("Alerts are disabled in settings, going back to sleep.");
                    }

                    _logger.LogInformation($"Alert service sleeping for {_settings.Monitoring.AlertIntervalInSeconds} seconds");
                    await Task.Delay((_settings.Monitoring.AlertIntervalInSeconds * 1000), token);
                } 
                while (!token.IsCancellationRequested);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("User intiated service end");
            }
            catch(Exception ex)
            {
                _logger.LogError("Unhandled alert service error:");
                _logger.LogError(ex.Message);
            }
        }

        private async Task RunServiceAction(CancellationToken token)
        {
            _logger.LogInformation("Alert service action running.");

            var monitoredIps = await _monitoringApi.GetAllPollsAsync();
            var alertIps = new List<IP>();

            foreach (var ip in monitoredIps)
            {
                var list = new List<IP>() { ip };
                var states = MonitorState.GetAllDevicePollsFromIps(list);

                var timespan = TimeSpan.FromMinutes(_alertEvalTimeInMinutes);
                var oldDate = DateTime.UtcNow - timespan;

                var pingStates = states
                    .Where(x => x.PingState != null)
                    .Where(x => x.SubmitTime > oldDate)
                    .OrderBy(x => x.SubmitTime)
                    .ToList();

                int totalCount = pingStates.Count();
                int upCount = pingStates.Where(x => x.PingState!.Response == true).ToList().Count();

                if (totalCount <= 0)
                {
                    totalCount = 1;
                }

                float uptimePercent = (upCount / totalCount) * 100;

                if (uptimePercent < _settings.Monitoring.AlertIfDownForPercent)
                {
                    _logger.LogWarning($"Alert: IP {IP.ConvertToString(ip.Address)} has uptime percent {uptimePercent}% which is below the threshold of {_settings.Monitoring.AlertIfDownForPercent}%");
                    alertIps.Add(ip);
                }
            }

            if (alertIps.Count > 0)
            {
                _logger.LogInformation($"Alert service submitting {alertIps.Count} IP addresses for alert consideration.");

                if (_settings.Smtp.AlertsEnabled)
                {
                    try
                    {
                        await _mailApi.SendAlertEmailAsync(alertIps);
                    }
                    catch { }
                }

                if (_settings.Discord.AlertsEnabled)
                {
                    try
                    {
                        var report = MonitorState.GetDowntimeAlertFromIps(alertIps);
                        await _discordService.SendMessageAsync(report);
                    }
                    catch(Exception ex) { _logger.LogError(ex, ex.Message); }
                }

                if (_settings.Telegram.AlertsEnabled)
                {
                    try
                    {
                        var report = MonitorState.GetDowntimeAlertFromIps(alertIps);
                        await _telegramService.SendMessageAsync(report);
                    }
                    catch (Exception ex) { _logger.LogError(ex, ex.Message); }
                }
            }
        }

        public async void Restart()
        {
            _logger.LogInformation("Alert service restart initiated");
            _cancellationToken.Cancel();
            _cancellationToken = new();
        }

        override public void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
