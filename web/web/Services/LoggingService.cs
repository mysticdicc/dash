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

        public LoggingService(
            IDbContextFactory<DashDbContext> dbContextFactory, 
            IHubContext<LogHub> logHub, 
            IServiceProvider sp)
        {
            _settings = sp.GetService<SettingsService>();
            _dbFactory = dbContextFactory;
            _logHub = logHub;
            _sp = sp;
        }

        public void LogWarning(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Warning, source, message);
            AddEntry(entry);
        }

        public async void LogWarningAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Warning, source, message);
            await AddEntryAsync(entry);
        }

        public void LogError(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Error, source, message);
            AddEntry(entry);
        }

        public async void LogErrorAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Error, source, message);
            await AddEntryAsync(entry);
        }

        public void LogInfo(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Info, source, message);
            AddEntry(entry);
        }

        public async void LogInfoAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Info, source, message);
            await AddEntryAsync(entry);
        }

        public bool CheckLogEntryAgainstSettings(LogEntry entry)
        {
            if (null == _settings)
            {
                try
                {
                    _settings = _sp.GetRequiredService<SettingsService>();

                    if (null == _settings) return false;
                }
                catch
                {
                    return false;
                }
            }

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
            var submit = CheckLogEntryAgainstSettings(entry);
            if (!submit) return;

            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.LogEntries.AddAsync(entry);
            await ctx.SaveChangesAsync();
            await _logHub.Clients.All.SendAsync("ReceiveLog", entry);
        }

        private void AddEntry(LogEntry entry)
        {
            var submit = CheckLogEntryAgainstSettings(entry);
            if (!submit) return;

            using var ctx = _dbFactory.CreateDbContext();
            ctx.LogEntries.Add(entry);
            ctx.SaveChanges();
            _logHub.Clients.All.SendAsync("ReceiveLog", entry);
        }

    }
}
