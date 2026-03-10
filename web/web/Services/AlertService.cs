using DashComponents.Monitoring;
using DashLib.API;
using DashLib.DankAPI;
using DashLib.Interfaces;
using DashLib.Interfaces.Monitoring;
using DashLib.Models;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using Newtonsoft.Json.Linq;
using System.Text;

namespace web.Services
{
    public class AlertService : BackgroundService, IDisposable
    {
        private Timer? _timer;
        private LoggingService _logger;
        CancellationTokenSource _cancellationToken;
        MonitoringAPI _monitoringApi;
        MailAPI _mailApi;
        DiscordService _discordService;
        TelegramService _telegramService;
        SettingsService _settings;
        private int _alertEvalTimeInMinutes;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.AlertService;

        public AlertService(
            LoggingService logger, 
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

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInfo("Service action has intiated", _logSource);

            while (!token.IsCancellationRequested)
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
                            _logger.LogInfo("Alerts are disabled in settings, going back to sleep.", _logSource);
                        }

                        _logger.LogInfo($"Alert service sleeping for {_settings.Monitoring.AlertIntervalInSeconds} seconds", _logSource);
                        await Task.Delay((_settings.Monitoring.AlertIntervalInSeconds * 1000), token);
                    }
                    while (!token.IsCancellationRequested);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInfo("User intiated service end", _logSource);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, _logSource);
                }
            }
        }

        private async Task RunServiceAction(CancellationToken token)
        {
            _logger.LogInfo("Alert service action running.", _logSource);

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
                    _logger.LogWarning($"Alert: IP {IP.ConvertToString(ip.Address)} has uptime percent {uptimePercent}% which is below the threshold of {_settings.Monitoring.AlertIfDownForPercent}%", _logSource);
                    alertIps.Add(ip);
                }
            }

            if (alertIps.Count > 0)
            {
                _logger.LogInfo($"Alert service submitting {alertIps.Count} IP addresses for alert consideration.", _logSource);

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
                    catch(Exception ex) { _logger.LogError(ex.Message, _logSource); }
                }

                if (_settings.Telegram.AlertsEnabled)
                {
                    try
                    {
                        var report = MonitorState.GetDowntimeAlertFromIps(alertIps);
                        await _telegramService.SendMessageAsync(report);
                    }
                    catch (Exception ex) { _logger.LogError(ex.Message, _logSource); }
                }
            }
        }

        public async void Restart()
        {
            _logger.LogInfo("Alert service restart initiated", _logSource);
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
