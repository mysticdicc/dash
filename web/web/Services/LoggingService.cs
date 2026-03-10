using dankweb.API;
using DashLib.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using web.Hubs;

namespace web.Services
{
    public class LoggingService
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory;
        private readonly IHubContext<LogHub> _logHub;
        private SettingsService? _settings;
        private readonly IServiceProvider _sp;
        private readonly SemaphoreSlim _semaphore = new(0, 1);
        private static readonly LogEntry.LogSource _logSource = LogEntry.LogSource.LoggingService;

        public LoggingService(
            IDbContextFactory<DashDbContext> dbContextFactory, 
            IHubContext<LogHub> logHub, 
            IServiceProvider sp)
        {
            _settings = null;
            _dbFactory = dbContextFactory;
            _logHub = logHub;
            _sp = sp;
        }

        public void SignalSettingsReady(SettingsService settings)
        {
            _settings = settings;
            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
            LogInfoAsync("Settings service has initiated ready.", _logSource);
        }

        public void LogWarning(string message, LogEntry.LogSource source) => LogWarningAsync(message, source);
        public void LogError(string message, LogEntry.LogSource source) => LogErrorAsync(message, source);
        public void LogInfo(string message, LogEntry.LogSource source) => LogInfoAsync(message, source);

        public async void LogWarningAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Warning, source, message);
            await AddEntryAsync(entry);
        }

        public async void LogErrorAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Error, source, message);
            await AddEntryAsync(entry);
        }

        public async void LogInfoAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Info, source, message);
            await AddEntryAsync(entry);
        }

        private async Task HandleSemaphore(LogEntry.LogSource source)
        {
            if (source != LogEntry.LogSource.SettingsService && source != LogEntry.LogSource.LoggingService)
            {
                await _semaphore.WaitAsync();
                _semaphore.Release();
            }
        }

        public bool CheckLogEntryAgainstSettings(LogEntry entry)
        {
            if (null == _settings) return true;

            if (entry.Source == LogEntry.LogSource.AlertService && !_settings.Logs.LogAlertEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.MonitoringService && !_settings.Logs.LogMonitoringEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.DiscordService && !_settings.Logs.LogDiscordEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.TelegramService && !_settings.Logs.LogTelegramEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.SettingsService && !_settings.Logs.LogSettingsEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.DiscoveryService && !_settings.Logs.LogDiscoveryEventsEnabled) return false;
            if (entry.Source == LogEntry.LogSource.ApiController && !_settings.Logs.LogApiEventsEnabled) return false;

            if (entry.Level == LogEntry.LogLevel.Info && !_settings.Logs.LogInformationEnabled) return false;
            if (entry.Level == LogEntry.LogLevel.Warning && !_settings.Logs.LogWarningEnabled) return false;
            if (entry.Level == LogEntry.LogLevel.Error && !_settings.Logs.LogErrorEnabled) return false;

            return true;
        }

        private async Task AddEntryAsync(LogEntry entry)
        {
            await HandleSemaphore(entry.Source);
            try
            {
                var submit = CheckLogEntryAgainstSettings(entry);
                if (!submit) return;

                using var ctx = await _dbFactory.CreateDbContextAsync();
                await ctx.LogEntries.AddAsync(entry);
                await ctx.SaveChangesAsync();
                await _logHub.Clients.All.SendAsync("ReceiveLog", entry);
            }
            catch(Exception outerEx)
            {
                Console.WriteLine("===== LOGGING SERVICE ERROR =====");
                Console.WriteLine(outerEx.Message);

                try
                {
                    using var ctx = await _dbFactory.CreateDbContextAsync();
                    var error = new LogEntry(LogEntry.LogLevel.Error, _logSource, "Error adding log entry: " + outerEx.Message);
                    await ctx.LogEntries.AddAsync(error);
                    await ctx.SaveChangesAsync();
                    await _logHub.Clients.All.SendAsync("ReceiveLog", error);
                }
                catch { }
            }
        }
    }
}
