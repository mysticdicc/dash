using dankweb.API;
using DashLib.Models;
using Microsoft.EntityFrameworkCore;

namespace web.Services
{
    public class CleanupService(LoggingService logger, SettingsService settingsService, IDbContextFactory<DashDbContext> dbContext) : BackgroundService
    {
        private readonly LoggingService _logger = logger;
        private readonly SettingsService _settings = settingsService;
        IDbContextFactory<DashDbContext> _dbContext = dbContext;
        private static readonly LogEntry.LogSource _logSource = LogEntry.LogSource.CleanupService;
        private TimeSpan _sleepTime = TimeSpan.FromHours(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("Service has started.", _logSource);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInfo("Service action execution started.", _logSource);

                try
                {
                    using var ctx = await _dbContext.CreateDbContextAsync();

                    var stateRetentionPeriod = TimeSpan.FromHours(_settings.Monitoring.MonitorStateRetentionPeriodInHours);
                    var stateDate = DateTime.Now - stateRetentionPeriod;
                    var states = await ctx.MonitorStates.Where(x => x.SubmitTime < stateDate).ToListAsync();
                    _logger.LogInfo($"{states.Count} monitor states are older than retention period and will therefore be deleted.", _logSource);

                    if (states.Count > 0)
                    {
                        ctx.MonitorStates.RemoveRange(states);
                    }

                    var logRetentionPeriod = TimeSpan.FromHours(_settings.Logs.RetentionPeriodHours);
                    var logDate = DateTime.Now - logRetentionPeriod;
                    var logs = await ctx.LogEntries.Where(x => x.Timestamp < logDate).ToListAsync();
                    _logger.LogInfo($"{logs.Count} logs are older than retention period and will therefore be deleted.", _logSource);

                    if (logs.Count > 0)
                    {
                        ctx.LogEntries.RemoveRange(logs);
                    }

                    await ctx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Service has soft crashed: " + ex.Message, _logSource);
                }

                _logger.LogInfo($"Cleanup task finished, sleeping for {_sleepTime}.", _logSource);
                await Task.Delay(_sleepTime);
            }
        }
    }
}
