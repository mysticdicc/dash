using dankweb.API;
using DashLib.Interfaces.Logging;
using DashLib.Models;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class LoggingRepository(IDbContextFactory<DashDbContext> dbContext) : ILoggingRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbContext = dbContext;

        public async Task<bool> DeleteAllLogsAsync()
        {
            using var ctx = await _dbContext.CreateDbContextAsync();
            var logs = await ctx.LogEntries.ToListAsync();
            ctx.LogEntries.RemoveRange(logs);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<IReadOnlyList<LogEntry>> GetAllLogsAsync()
        {
            using var ctx = await  _dbContext.CreateDbContextAsync();
            var logs = await ctx.LogEntries.ToListAsync();
            return logs;
        }
    }
}
