using DashLib.DankAPI;
using DashLib.Models;
using DashLib.Models.Settings;
using DashLib.Models.Settings.Monitoring;

namespace web.Services
{
    public class SettingsService : IDisposable, IHostedService
    {
        public AllSettings All { get; set; }
        public DiscordSettings Discord => All.MonitoringSettings.DiscordSettings;
        public MonitoringSettings Monitoring => All.MonitoringSettings;
        public TelegramSettings Telegram => All.MonitoringSettings.TelegramSettings;
        public DashboardSettings Dashboard => All.DashboardSettings;
        public SubnetSettings Subnet => All.SubnetSettings;
        public SmtpSettings Smtp => All.MonitoringSettings.SmtpSettings;
        public LoggingSettings Logs => All.LoggingSettings;

        private int retryCount = 3;
        private int retryDelay = 2000;
        private int refreshInterval = 15;

        private PeriodicTimer? _timer;
        private Task? _timerTask;
        private CancellationTokenSource? _cts;

        private readonly LoggingService _logger;
        private static readonly LogEntry.LogSource _logSource = LogEntry.LogSource.SettingsService;

        public SettingsService(LoggingService logger)
        {
            All = new AllSettings(true);
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(refreshInterval));
            _timerTask = RefreshSettingsAsync(_cts.Token);
            _logger.SignalSettingsReady(this);
            await Task.Delay(2500); //wait for logging service to start
            _logger.LogInfo("Service started.", _logSource);
        }
          
        async public Task RefreshSettingsAsync(CancellationToken token)
        {
            _logger.LogInfo("Refresh settings task initiated.", _logSource);
            AllSettings? settings = null;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);
                    if (settings != null)
                    {
                        _logger.LogInfo($"Attempt {i+1}: Succeeding in fetching or creating default settings file.", _logSource);
                        break;
                    }
                }
                catch(Exception ex) 
                {
                    _logger.LogError($"Attempt {i + 1}: Error fetching or creating default settings file: " + ex.Message, _logSource);
                }

                await Task.Delay(retryDelay);
            }

            if (settings == null)
            {
                _logger.LogWarning($"Failed to fetch or create settings file after {retryCount} attempts. Loading default settings to memory.", _logSource);
                All = new AllSettings(true);

                try
                {
                    AllSettings.CreateNewSettingsFile(AllSettings.SettingsPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to write settings from memory to new file: " + ex.Message, _logSource);
                }
            }
            else
            {
                All = settings;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Service stop initiated.", _logSource);
            _cts?.Cancel();
            if (_timerTask != null)
            {
                await _timerTask;
            }
        }

        public void Dispose()
        {
            _logger.LogInfo("Service dispose initiated.", _logSource);
            _timer?.Dispose();
        }
    }
}
