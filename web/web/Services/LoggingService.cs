using dankweb.API;
using DashLib.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using web.Hubs;

namespace web.Services
{
    public class LoggingService(IDbContextFactory<DashDbContext> dbContextFactory, IHubContext<LogHub> logHub)
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContextFactory;
        private readonly IHubContext<LogHub> _logHub = logHub;

        public void LogWarning(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Warning, source, message);
            AddEntry(entry);
        }

        public async Task LogWarningAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Warning, source, message);
            await AddEntryAsync(entry);
        }

        public void LogError(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Error, source, message);
            AddEntry(entry);
        }

        public async Task LogErrorAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Error, source, message);
            await AddEntryAsync(entry);
        }

        public void LogInfo(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Info, source, message);
            AddEntry(entry);
        }

        public async Task LogInfoAsync(string message, LogEntry.LogSource source)
        {
            var entry = new LogEntry(LogEntry.LogLevel.Info, source, message);
            await AddEntryAsync(entry);
        }

        private async Task AddEntryAsync(LogEntry entry)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.LogEntries.AddAsync(entry);
            await ctx.SaveChangesAsync();
            await _logHub.Clients.All.SendAsync("ReceiveLog", entry);
        }

        private void AddEntry(LogEntry entry)
        {
            using var ctx = _dbFactory.CreateDbContext();
            ctx.LogEntries.Add(entry);
            ctx.SaveChanges();
            _logHub.Clients.All.SendAsync("ReceiveLog", entry);
        }

    }
}
