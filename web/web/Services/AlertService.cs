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
using Org.BouncyCastle.Asn1.X509;
using System.Text;

namespace web.Services
{
    public class AlertService : BackgroundService, IDisposable
    {
        private Timer? _timer;
        private LoggingService _logger;
        CancellationTokenSource _cancellationToken;
        IMonitorStateRepository _monitoringRepo;
        DiscordService _discordService;
        TelegramService _telegramService;
        SettingsService _settings;
        MailService _mailService;
        private int _alertEvalTimeInMinutes;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.AlertService;

        public AlertService(
            LoggingService logger, 
            IMonitorStateRepository monitoringRepo,
            MailService mailService, 
            DiscordService discordAPI,
            SettingsService settingsService,
            TelegramService telegramService)
        {
            _logger = logger;
            _monitoringRepo = monitoringRepo;
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
                        var list = new List<BaseMonitoringTarget>();

                        var monitoredIps = await _monitoringRepo.GetMonitoredIpAndStatusAsync();
                        list.AddRange(monitoredIps);

                        var monitoredDns = await _monitoringRepo.GetMonitoredDnsAndStatusAsync();
                        list.AddRange(monitoredDns);

                        if (_settings.Alerts.IcmpDownPercentAlertsEnabled) 
                            await IcmpDownOverTimeReportAsync(token, list);
                        if (_settings.Alerts.IcmpDownOnceAlertsEnabled) 
                            await IcmpDownOnceReportAsync(token, list);
                        if (_settings.Alerts.TcpDownPercentAlertsEnabled)
                            await TcpDownOverTimeReportAsync(token, list);
                        if (_settings.Alerts.TcpDownOnceAlertsEnabled)
                            await TcpDownOnceReportAsync(token, list);

                        _logger.LogInfo($"Alert service sleeping for {_settings.Alerts.AlertIntervalInSeconds} seconds", _logSource);
                        await Task.Delay((_settings.Alerts.AlertIntervalInSeconds * 1000), token);
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
                    await Task.Delay((_settings.Alerts.AlertIntervalInSeconds * 1000), token);
                }
            }
        }

        private async Task IcmpDownOverTimeReportAsync(CancellationToken token, List<BaseMonitoringTarget> monitoringTargets)
        {
            _logger.LogInfo("Icmp down over time report action running.", _logSource);

            var alertTargets = new List<BaseMonitoringTarget>();
            var allStates = PingState.GetAllMonitorStatesFromListMonitoringTargets(monitoringTargets);
            var grouped = allStates.GroupBy(x => x.TargetId);

            foreach (var group in grouped)
            {
                var timespan = TimeSpan.FromMinutes(_alertEvalTimeInMinutes);
                var uptimePercent = PingState.CalculateUptimePercentage(timespan, group.ToList());

                if (uptimePercent < _settings.Alerts.AlertIfDownForPercent)
                {
                    var target = monitoringTargets.Where(x => x.Id == group.Key).First();

                    if (null == target)
                    {
                        _logger.LogError($"Monitoring service detected device ID: {group.Key} below threshold but alert will not be sent due to missing device info.", _logSource);
                    }
                    else
                    {
                        _logger.LogWarning($"Alert: Monitoring target {target.Hostname} has uptime percent {uptimePercent}% on ping which is below the threshold of {_settings.Alerts.AlertIfDownForPercent}%", _logSource);
                        alertTargets.Add(target);
                    }
                }
            }

            if (alertTargets.Count > 0)
            {
                await SendDownOverTimeAlertAsync(token, alertTargets, true);
            }
        }

        private async Task IcmpDownOnceReportAsync(CancellationToken token, List<BaseMonitoringTarget> monitoringTargets)
        {
            _logger.LogInfo("Icmp down once report action running.", _logSource);

            var last = PingState.GetMostRecentStatesFromListMonitoringTargets(monitoringTargets);
            var down = last.Where(x => !x.Response).ToList();
            var targets = down.Select(x => x.Target).Distinct().ToList();

            if (targets == null) return;
            if (targets.Count() > 0) await SendDownOnceAlertsAsync(token, targets, true);
        }

        private async Task TcpDownOverTimeReportAsync(CancellationToken token, List<BaseMonitoringTarget> monitoringTargets)
        {
            _logger.LogInfo("Tcp down over time report action running.", _logSource);

            var alertTargets = new List<BaseMonitoringTarget>();
            var allStates = PortState.GetAllMonitorStatesFromListMonitoringTargets(monitoringTargets);
            var targetGroups = allStates.GroupBy(x => (x.Target, x.TargetPort));

            foreach (var targetGroup in targetGroups)
            {
                var timespan = TimeSpan.FromMinutes(_alertEvalTimeInMinutes);
                var uptimePercent = PortState.CalculateUptimePercentage(timespan, targetGroup.ToList());

                if (uptimePercent < _settings.Alerts.AlertIfDownForPercent)
                {
                    _logger.LogWarning($"Alert: Monitoring target {targetGroup.Key.Target.Hostname} has uptime percent {uptimePercent}% on port {targetGroup.Key.TargetPort} which is below the threshold of {_settings.Alerts.AlertIfDownForPercent}%", _logSource);
                    alertTargets.Add(targetGroup.Key.Target);
                }
            }

            if (alertTargets.Count > 0)
            {
                await SendDownOverTimeAlertAsync(token, alertTargets, false);
            }
        }

        private async Task TcpDownOnceReportAsync(CancellationToken token, List<BaseMonitoringTarget> monitoringTargets)
        {
            _logger.LogInfo("Tcp down over time report action running.", _logSource);

            var last = PortState.GetMostRecentStatesFromListMonitoringTargets(monitoringTargets);
            var down = last.Where(x => !x.Response).ToList();
            var targets = down.Select(x => x.Target).Distinct().ToList();

            if (targets == null) return;
            if (targets.Count() > 0) await SendDownOnceAlertsAsync(token, targets, false);
        }

        private async Task SendAlertsAsync(CancellationToken token, string message)
        {
            if (_settings.Smtp.AlertsEnabled)
            {
                try
                {
                    await _mailService.SendMailAsync("Downtime Alert", message);
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
                    await _discordService.SendMessageAsync(message);
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
                    await _telegramService.SendMessageAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error telegram mail alert: " + ex.Message, _logSource);
                }
            }
        }

        private async Task SendDownOverTimeAlertAsync(CancellationToken token, List<BaseMonitoringTarget> alertTargets, bool icmp)
        {
            alertTargets = alertTargets.Distinct().ToList();
            _logger.LogInfo($"Submitting {alertTargets.Count} monitoring targets for down over time alerts..", _logSource);

            var sb = new StringBuilder();
            sb.AppendLine("Device Down Over Time Alert");
            sb.AppendLine($"Below Threshold: {alertTargets.Count}");
            sb.AppendLine();

            foreach (var target in alertTargets)
            {
                if (target is DnsMonitoringTarget dns)
                {
                    sb.AppendLine($"Hostname: {dns.Hostname}");
                    sb.AppendLine($"Address: {dns.Address}");
                }
                else if (target is IpMonitoringTarget ip)
                {
                    sb.AppendLine($"Hostname: {ip.Hostname}");
                    sb.AppendLine($"Address: {IpMonitoringTarget.ConvertToString(ip.Address)}");
                }

                var timespan = TimeSpan.FromMinutes(_alertEvalTimeInMinutes);

                if (icmp)
                {
                    var pings = PingState.GetAllMonitorStatesFromMonitoringTarget(target);
                    var uptime = PingState.CalculateUptimePercentage(timespan, pings);

                    sb.AppendLine($"Icmp Uptime Percentage: {uptime}%");
                }
                else
                {
                    var ports = PortState.GetAllMonitorStatesFromMonitoringTarget(target);
                    var grouped = ports.GroupBy(x => (x.Target, x.TargetPort));

                    foreach (var group in grouped)
                    {
                        var uptime = PortState.CalculateUptimePercentage(timespan, group.ToList());

                        sb.AppendLine($"Tcp Port: {group.Key.TargetPort}");
                        sb.AppendLine($"Uptime Percentage: {uptime}%");
                    }
                }
            }

            await SendAlertsAsync(token, sb.ToString());
        }

        private async Task SendDownOnceAlertsAsync(CancellationToken token, List<BaseMonitoringTarget> alertTargets, bool icmp)
        {
            alertTargets = alertTargets.Distinct().ToList();
            _logger.LogInfo($"Submitting {alertTargets.Count} monitoring targets for immediate downtime notification.", _logSource);

            foreach (var target in alertTargets)
            {
                var sb = new StringBuilder();

                if (icmp)
                {
                    sb.AppendLine("No Ping Response Alert:");
                }
                else
                {
                    sb.AppendLine("No Tcp Response Alert:");
                }

                if (target is DnsMonitoringTarget dns)
                {
                    sb.AppendLine($"Hostname: {dns.Hostname}");
                    sb.AppendLine($"Address: {dns.Address}");
                }
                else if (target is IpMonitoringTarget ip)
                {
                    sb.AppendLine($"Hostname: {ip.Hostname}");
                    sb.AppendLine($"Address: {IpMonitoringTarget.ConvertToString(ip.Address)}");
                }
                
                if (icmp)
                {
                    var ping = PingState.GetMostRecentStateFromMonitoringTarget(target).First();

                    if (null != ping)
                    {
                        sb.AppendLine($"Last ping status: {ping.Response}");
                    }
                }
                else
                {
                    var ports = PortState.GetMostRecentStateFromMonitoringTarget(target);

                    if (null != ports)
                    {
                        foreach (var port in ports)
                        {
                            if (target.TcpPortsMonitored.Contains(port.TargetPort))
                            {
                                sb.AppendLine($"Port: {port.TargetPort}");
                                sb.AppendLine($"Port Status: {port.Response}");
                            }
                        }
                    }
                }

                await SendAlertsAsync(token, sb.ToString());
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
