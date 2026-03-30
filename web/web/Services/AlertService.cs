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
        DiscordService _discordService;
        TelegramService _telegramService;
        SettingsService _settings;
        MailService _mailService;
        private int _alertEvalTimeInMinutes;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.AlertService;

        public AlertService(
            LoggingService logger, 
            MonitoringAPI monitoringApi,
            MailService mailService, 
            DiscordService discordAPI,
            SettingsService settingsService,
            TelegramService telegramService)
        {
            _logger = logger;
            _monitoringApi = monitoringApi;
            _mailService = mailService;
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
                        var monitoredIps = await _monitoringApi.GetMonitoredIpsAsync();

                        if (_settings.Monitoring.IcmpDownPercentAlertsEnabled) 
                            await IcmpDownOverTimeReportAsync(token, monitoredIps);
                        if (_settings.Monitoring.IcmpDownOnceAlertsEnabled) 
                            await IcmpDownOnceReportAsync(token, monitoredIps);
                        if (_settings.Monitoring.TcpDownPercentAlertsEnabled)
                            await TcpDownOverTimeReportAsync(token, monitoredIps);
                        if (_settings.Monitoring.TcpDownOnceAlertsEnabled)
                            await TcpDownOnceReportAsync(token, monitoredIps);

                        var monitoredDns = await _monitoringApi.Get

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

        private async Task IcmpDownOverTimeReportAsync(CancellationToken token, List<IpMonitoringTarget> allPolls)
        {
            _logger.LogInfo("Icmp down over time report action running.", _logSource);
            var alertIps = new List<IpMonitoringTarget>();

            foreach (var ip in allPolls)
            {
                var list = new List<IpMonitoringTarget>() { ip };
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
                    _logger.LogWarning($"Alert: IP {IpMonitoringTarget.ConvertToString(ip.Address)} has uptime percent {uptimePercent}% which is below the threshold of {_settings.Monitoring.AlertIfDownForPercent}%", _logSource);
                    alertIps.Add(ip);
                }
            }

            if (alertIps.Count > 0)
            {
                await SendDowntimeAlertAsync(token, alertIps);
            }
        }

        private async Task IcmpDownOnceReportAsync(CancellationToken token, List<IpMonitoringTarget> allPolls)
        {
            var lastPolls = PingState.(allPolls);
            var icmpPolls = lastPolls.Where(x => x.PingState != null && !x.PingState.Response).ToList();
            var ips = icmpPolls.Select(x => x.IP).Distinct().ToList();

            if (ips == null) return;

            if (ips.Count() > 0)
            {
                await SendDowntimeAlertAsync(token, ips!);
            }
        }

        private async Task TcpDownOverTimeReportAsync(CancellationToken token, List<IP> allPolls)
        {

        }

        private async Task TcpDownOnceReportAsync(CancellationToken token, List<IP> allPolls)
        {

        }

        private async Task SendAlertsAsync(CancellationToken token, List<IP> alertIps, string message)
        {

        }

        private async Task SendDowntimeAlertAsync(CancellationToken token, List<IP> alertIps)
        {
            _logger.LogInfo($"Alert service submitting {alertIps.Count} IP addresses for alert consideration.", _logSource);
            var report = MonitorState.GetDowntimeAlertFromIps(alertIps);

            if (_settings.Smtp.AlertsEnabled)
            {
                try
                {
                    await _mailService.SendMailAsync("Downtime Alert", report);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error sending mail alert: " + ex.Message, _logSource);
                }
            }

            if (_settings.Discord.AlertsEnabled)
            {
                try
                {
                    await _discordService.SendMessageAsync(report);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error discord mail alert: " + ex.Message, _logSource);
                }
            }

            if (_settings.Telegram.AlertsEnabled)
            {
                try
                {
                    await _telegramService.SendMessageAsync(report);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error telegram mail alert: " + ex.Message, _logSource);
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
