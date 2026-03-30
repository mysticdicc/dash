using dankweb.API;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using Microsoft.EntityFrameworkCore;
using web.Services;

namespace web.Data.Repos
{
    public class MonitorStateRepository(IDbContextFactory<DashDbContext> dbContext) : IMonitorStateRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        public async Task<IReadOnlyList<IpMonitoringTarget>> GetMonitoredIpAndStatusAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var ipList = await ctx.IpTargets
                .Where(x => x.IsMonitoredIcmp || x.IsMonitoredTcp)
                .Include(x => x.TcpMonitorStates)
                .Include(x => x.IcmpMonitorStates)
                .ToListAsync();

            return ipList;
        }

        public async Task<IReadOnlyList<IpMonitoringTarget>> GetAllMonitoredIpsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var ipList = await ctx.IpTargets
                .Where(x => x.IsMonitoredIcmp || x.IsMonitoredTcp)
                .ToListAsync();

            return ipList;
        }

        public async Task<IReadOnlyList<BaseMonitorState>> GetMonitorStatesByDeviceIdAsync(int ID)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var list = new List<BaseMonitorState>();

            var tcp = await ctx.PortStates.Where(x => x.TargetId == ID).ToListAsync();
            if (tcp.Count > 0)
            {
                list.AddRange(tcp);
            }

            var icmp = await ctx.PingStates.Where(x => x.TargetId == ID).ToListAsync();
            if (icmp.Count > 0)
            {
                list.AddRange(icmp);
            }

            return list;
        }

        public async Task<bool> AddMonitorStatesFromListIpAsync(List<IpMonitoringTarget> ips)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            foreach (var ip in ips)
            {
                if (ip.IcmpMonitorStates.Count > 0)
                {
                    await ctx.PingStates.AddRangeAsync(ip.IcmpMonitorStates);
                }

                if (ip.TcpMonitorStates.Count > 0)
                {
                    await ctx.PortStates.AddRangeAsync(ip.TcpMonitorStates);
                }
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<IReadOnlyList<PortState>> GetAllPortStatesAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.PortStates.ToListAsync();
            return list;
        }

        public async Task<IpMonitoringTarget> GetIpMonitoringTargetByStringAddressAsync(string address)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var byteIp = IpMonitoringTarget.ConvertToByte(address);
            var entity = await ctx.IpTargets.Where(x => x.Address.SequenceEqual(byteIp))
                .Include(x => x.TcpMonitorStates)
                .Include(x => x.IcmpMonitorStates)
                .FirstOrDefaultAsync();

            if (null == entity) throw new InvalidDataException($"No entity with IP: {address}");

            return entity;
        }

        public async Task<IReadOnlyList<DnsMonitoringTarget>> GetMonitoredDnsAndStatusAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var dnsList = await ctx.DnsTargets
                .Where(x => x.IsMonitoredIcmp || x.IsMonitoredTcp)
                .Include(x => x.TcpMonitorStates)
                .Include(x => x.IcmpMonitorStates)
                .ToListAsync();

            return dnsList;
        }

        public async Task<IReadOnlyList<DnsMonitoringTarget>> GetAllMonitoredDnsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var dnsList = await ctx.DnsTargets
                .Where(x => x.IsMonitoredIcmp || x.IsMonitoredTcp)
                .ToListAsync();

            return dnsList;
        }

        public async Task<bool> AddMonitorStatesFromListDnsAsync(List<DnsMonitoringTarget> dnsList)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            foreach (var dns in dnsList)
            {
                if (dns.IcmpMonitorStates.Count > 0)
                {
                    await ctx.PingStates.AddRangeAsync(dns.IcmpMonitorStates);
                }

                if (dns.TcpMonitorStates.Count > 0)
                {
                    await ctx.PortStates.AddRangeAsync(dns.TcpMonitorStates);
                }
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<IReadOnlyList<PingState>> GetAllPingStatesAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var states = await ctx.PingStates.ToListAsync();
            return states;
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitoringTargetByStringAddressAsync(string address)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var entity = await ctx.DnsTargets.Where(x => x.Address == address)
                .Include(x => x.TcpMonitorStates)
                .Include(x => x.IcmpMonitorStates)
                .FirstOrDefaultAsync();

            if (null == entity) throw new InvalidDataException($"No entity with addresss: {address}");

            return entity;
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitorStatesByDeviceIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var entity = await ctx.DnsTargets.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (null == entity) throw new InvalidDataException($"No entity with ID: {id}");
            return entity;
        }

        public async Task<IpMonitoringTarget> GetIpMonitorStatesByDeviceIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();

            var entity = await ctx.IpTargets.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (null == entity) throw new InvalidDataException($"No entity with ID: {id}");
            return entity;
        }
    }
}