using DashComponents.Monitoring;
using DashLib.DankAPI;
using DashLib.Settings;
using DashLib.Monitoring;
using DashLib.Network;
using DashLib.API;

namespace web.Services
{
    public class AlertService : BackgroundService, IDisposable
    {
        private Timer? _timer;
        public int _delay;
        private ILogger<MonitorService> _logger;
        CancellationTokenSource _cancellationToken;
        MonitoringAPI _monitoringApi;
        MailAPI _mailApi;
        AllSettings _currentSettings;
        private float _alertPercent;
        private bool _alertsEnabled;
        private int _alertEvalTimeInMinutes;

        public AlertService(ILogger<MonitorService> logger, MonitoringAPI monitoringApi, MailAPI mailApi)
        {
            _logger = logger;
            _monitoringApi = monitoringApi;
            _mailApi = mailApi;
            _cancellationToken = new CancellationTokenSource();

            try
            {
                _logger.LogInformation("Fetching current settings");
                _currentSettings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to fetch current settings: ");
                _logger.LogError(ex.Message);
                _logger.LogInformation("Creating new settings file");

                try
                {
                    AllSettings.CreateNewSettingsFile(AllSettings.SettingsPath);
                    _currentSettings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);
                }
                catch (Exception ex2)
                {
                    _logger.LogError("Failed to create new settings:");
                    _logger.LogError(ex2.Message);

                    _logger.LogInformation("Using default settings");
                    _currentSettings = new AllSettings(true);
                }
            }

            if (null != _currentSettings.MonitoringSettings)
            {
                _delay = _currentSettings.MonitoringSettings.AlertIntervalInSeconds;
                _alertPercent = _currentSettings.MonitoringSettings.AlertIfDownForPercent;
                _alertsEnabled = _currentSettings.MonitoringSettings.AlertsEnabled;
                _alertEvalTimeInMinutes = _currentSettings.MonitoringSettings.AlertTimePeriodInMinutes;
            }
            else
            {
                _delay = 600;
                _alertPercent = 50.0F;
                _alertsEnabled = false;
                _alertEvalTimeInMinutes = 60;
                _logger.LogWarning("Monitoring settings not found in settings file, using default values");
            }
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
                    _currentSettings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

                    if (null != _currentSettings.MonitoringSettings)
                    {
                        _delay = _currentSettings.MonitoringSettings.AlertIntervalInSeconds;
                        _alertPercent = _currentSettings.MonitoringSettings.AlertIfDownForPercent;
                        _alertsEnabled = _currentSettings.MonitoringSettings.AlertsEnabled;
                        _alertEvalTimeInMinutes = _currentSettings.MonitoringSettings.AlertTimePeriodInMinutes;
                    }
                    else
                    {
                        _logger.LogWarning("Monitoring settings not found in settings file, using previous values");
                    }

                    if (_alertsEnabled)
                    {
                        await RunServiceAction(token);
                    }
                    else
                    {
                        _logger.LogInformation("Alerts are disabled in settings, going back to sleep.");
                    }

                    _logger.LogInformation($"Alert service sleeping for {_delay} seconds");
                    await Task.Delay((_delay * 1000), token);
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

                if (upCount <= 0)
                {
                    upCount = 1;
                }

                if (totalCount <= 0)
                {
                    totalCount = 1;
                }

                float uptimePercent = (upCount / totalCount) * 100;

                if (uptimePercent < _alertPercent)
                {
                    _logger.LogWarning($"Alert: IP {IP.ConvertToString(ip.Address)} has uptime percent {uptimePercent}% which is below the threshold of {_alertPercent}%");
                    alertIps.Add(ip);
                }
            }

            if (alertIps.Count > 0)
            {
                _logger.LogInformation($"Alert service submitting {alertIps.Count} IP addresses for alert consideration.");
                await _mailApi.SendAlertEmailAsync(alertIps);
            }
        }

        public async void Restart()
        {
            _logger.LogInformation("Alert service restart initiated");
            _currentSettings = AllSettings.GetCurrentSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));

            if (null == _currentSettings.MonitoringSettings)
            {
                do
                {
                    _currentSettings = AllSettings.GetCurrentSettingsFile(Path.Combine(AppContext.BaseDirectory, "settings.json"));
                    _logger.LogError("Failed to fetch monitoring settings on alert servivce restart, retrying in 20 seconds.");
                    await Task.Delay(20000);
                }
                while (null == _currentSettings.MonitoringSettings);
            }

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
