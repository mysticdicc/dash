using dankweb.API;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel;

namespace web.Data.Repos
{
    public class MonitorTargetRepository(IDbContextFactory<DashDbContext> dbContext) : IMonitorTargetRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        public async Task<bool> AddDnsContainerAsync(DnsContainer container)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.DnsContainers.AddAsync(container);

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> AddDnsTargetAsync(DnsMonitoringTarget dns)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.DnsTargets.AddAsync(dns);

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> AddIpTargetAsync(IpMonitoringTarget ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.IpTargets.AddAsync(ip);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> AddSubnetContainerAsync(SubnetContainer subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.SubnetContainers.AddAsync(subnet);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteDnsContainerAsync(DnsContainer container)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsContainers.FirstOrDefaultAsync(x => x.Id == container.Id);

            if (entity == null) throw new InvalidDataException($"No DNS container with ID: {container.Id}");

            ctx.DnsContainers.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteDnsContainerByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsContainers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No DNS container with ID: {id}");

            ctx.DnsContainers.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteDnsTargetAsync(DnsMonitoringTarget target)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsTargets.FirstOrDefaultAsync(x => x.Id == target.Id);

            if (entity == null) throw new InvalidDataException($"No DNS target with ID: {target.Id}");

            ctx.DnsTargets.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteIpTargetAsync(IpMonitoringTarget ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.IpTargets.FirstOrDefaultAsync(x => x.Id == ip.Id);

            if (entity == null) throw new InvalidDataException($"No IP target with ID: {ip.Id}");

            ctx.IpTargets.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteSubnetByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.SubnetContainers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No subnet container with ID: {id}");

            ctx.SubnetContainers.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteSubnetContainerAsync(SubnetContainer subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.SubnetContainers.FirstOrDefaultAsync(x => x.Id == subnet.Id);

            if (entity == null) throw new InvalidDataException($"No subnet container with ID: {subnet.Id}");

            ctx.SubnetContainers.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<IReadOnlyList<DnsContainer>> GetAllDnsContainersAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.DnsContainers.Include(x => x.Children).ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<DnsMonitoringTarget>> GetAllDnsTargetsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.DnsTargets.ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<IpMonitoringTarget>> GetAllIpsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.IpTargets.ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<SubnetContainer>> GetAllSubnetContainersAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.SubnetContainers.Include(x => x.Children).ToListAsync();
            return list;
        }

        public async Task<DnsContainer> GetDnsContainerByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsContainers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No DNS container with ID: {id}");

            return entity;
        }

        public async Task<DnsMonitoringTarget> GetDnsTargetByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsTargets.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No DNS target with ID: {id}");

            return entity;
        }

        public async Task<IpMonitoringTarget> GetIpTargetByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.IpTargets.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No IP target with ID: {id}");

            return entity;
        }

        public async Task<SubnetContainer> GetSubnetContainerByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.SubnetContainers.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) throw new InvalidDataException($"No subnet container with ID: {id}");

            return entity;
        }

        public async Task<bool> SubmitDiscoveryTaskAsync(SubnetContainer subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.SubnetContainers.Include(x => x.Children).FirstOrDefaultAsync(x => x.Id == subnet.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {subnet.Id}");

            subnet.Children ??= [];
            entity.Children ??= [];

            var idDict = entity.Children.ToDictionary(x => x.Id, x => x);
            var listId = subnet.Children.Select(x => x.Id).ToList();

            foreach (var submitted in subnet.Children)
            {
                if (idDict.TryGetValue(submitted.Id, out var existing))
                {
                    if (submitted.Hostname != string.Empty) existing.Hostname = submitted.Hostname;
                    existing.IsMonitoredIcmp = submitted.IsMonitoredIcmp;
                }
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateDnsContainerAsync(DnsContainer container)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsContainers.Include(x => x.Children).FirstOrDefaultAsync(x => x.Id == container.Id);

            if (entity == null) throw new InvalidDataException($"No DNS container with ID: {container.Id}");

            entity.Children = container.Children;
            entity.DisplayName = container.DisplayName;

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateDnsTargetAsync(DnsMonitoringTarget target)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DnsTargets.FirstOrDefaultAsync(x => x.Id == target.Id);

            if (entity == null) throw new InvalidDataException($"No DNS target with ID: {target.Id}");

            entity.Address = target.Address;
            entity.Parent = target.Parent;
            entity.IsMonitoredIcmp = target.IsMonitoredIcmp;

            if (null != target.TcpPortsMonitored)
            {
                entity.IsMonitoredTcp = true;
                entity.TcpPortsMonitored = target.TcpPortsMonitored;
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateIpTargetAsync(IpMonitoringTarget ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.IpTargets.FirstOrDefaultAsync(x => x.Id == ip.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {ip.Id}");

            entity.Hostname = ip.Hostname;
            entity.IsMonitoredIcmp = ip.IsMonitoredIcmp;

            if (null != ip.TcpPortsMonitored)
            {
                entity.IsMonitoredTcp = true;
                entity.TcpPortsMonitored = ip.TcpPortsMonitored;
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateSubnetContainerAsync(SubnetContainer subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.SubnetContainers.FirstOrDefaultAsync(x => x.Id == subnet.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {subnet.Id}");

            entity.Children = subnet.Children;
            entity.Address = subnet.Address;
            entity.StartAddress = subnet.StartAddress;
            entity.EndAddress = subnet.EndAddress;
            entity.SubnetMask = subnet.SubnetMask;

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }
    }
}
