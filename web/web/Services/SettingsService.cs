using DashLib.DankAPI;
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

        private int retryCount = 3;
        private int retryDelay = 2000;
        private int refreshInterval = 15;

        private PeriodicTimer? _timer;
        private Task? _timerTask;
        private CancellationTokenSource? _cts;

        public SettingsService()
        {
            All = new AllSettings(true);
        }

        public async Task StartAsync(CancellationToken token)
        {
            await RefreshSettingsAsync(token);

            // Start background refresh
            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(refreshInterval));
            _timerTask = RefreshSettingsAsync(_cts.Token);
        }

        async public Task RefreshSettingsAsync(CancellationToken token)
        {
            AllSettings? settings = null;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);
                    if (settings != null)
                    {
                        break;
                    }
                }
                catch { }

                await Task.Delay(retryDelay);
            }

            if (settings == null)
            {
                All = new AllSettings();
                AllSettings.CreateNewSettingsFile(AllSettings.SettingsPath);
            }
            else
            {
                All = settings;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts?.Cancel();
            if (_timerTask != null)
            {
                await _timerTask;
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
