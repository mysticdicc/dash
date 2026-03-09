using dankweb.API;
using DashLib.Interfaces.Monitoring;
using DashLib.Monitoring;
using DashLib.Network;
using Microsoft.EntityFrameworkCore;
using web.Services;

namespace web.Data.Repos
{
    public class MonitoringRepository(IDbContextFactory<DashDbContext> dbContext) : IMonitoringRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        public async Task<IReadOnlyList<IP>> GetMonitoredDevicesAndStatusAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var list = await ctx.IPs
                .Where(x => (x.IsMonitoredTCP || x.IsMonitoredICMP) && x.MonitorStateList != null)
                .Include(x => x.MonitorStateList!)
                    .ThenInclude(x => x.PortState)
                .Include(x => x.MonitorStateList!)
                    .ThenInclude(x => x.PingState)
                .ToListAsync();

            return list;
        }

        public async Task<IReadOnlyList<IP>> GetAllMonitoredDevicesAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.IPs.Where(x => (x.IsMonitoredTCP || x.IsMonitoredICMP)).ToListAsync();

            return list;
        }

        public async Task<IReadOnlyList<MonitorState>> GetMonitorStatesByDeviceIdAsync(int ID)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var list = await ctx.MonitorStates.Where(x => x.IP_ID == ID)
                .Include(x => x.PortState)
                .Include(x => x.PingState)
                .ToListAsync();

            return list;
        }

        public async Task<bool> AddMonitorStatesFromListIpAsync(List<IP> ips)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var states = ips.SelectMany(ips => ips.MonitorStateList ?? []).ToList();
            await ctx.MonitorStates.AddRangeAsync(states);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<IReadOnlyList<MonitorState>> GetAllMonitorStatesAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.MonitorStates
                .Include(x => x.PortState)
                .Include(x => x.PingState)
                .ToListAsync();

            return list;
        }
    }
}