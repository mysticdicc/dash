using dankweb.API;
using DashLib.Interfaces.Network;
using DashLib.Models.Network;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class SubnetRepository(IDbContextFactory<DashDbContext> dbContext) : ISubnetRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        public async Task<bool> AddNewIpAsync(IP ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.IPs.AddAsync(ip);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> AddSubnetAsync(Subnet subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.Subnets.AddAsync(subnet);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteIpAsync(IP ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.IPs.FirstOrDefaultAsync(x => x.ID == ip.ID);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {ip.ID}");

            ctx.IPs.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteSubnetAsync(Subnet subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.Subnets.FirstOrDefaultAsync(x => x.ID == subnet.ID);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {subnet.ID}");

            ctx.Subnets.Remove(entity);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<IReadOnlyList<IP>> GetAllIpsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var ips = await ctx.IPs.ToListAsync();
            return ips;
        }

        public async Task<IReadOnlyList<Subnet>> GetAllSubnetsWithIpsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var subnets = await ctx.Subnets.Include(x => x.List).ToListAsync();
            return subnets;
        }

        public async Task<IP> GetIpByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var ip = await ctx.IPs.FirstOrDefaultAsync(x => x.ID == id);

            if (ip == null) throw new InvalidDataException($"No entity with ID: {id}");

            return ip;
        }

        public async Task<Subnet> GetSubnetByIdAsync(int id)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var subnet = await ctx.Subnets.Include(x => x.List).FirstOrDefaultAsync(x => x.ID == id);
            
            if (null == subnet) throw new InvalidDataException($"No entity with ID: {id}");

            return subnet;
        }

        public async Task<bool> SubmitDiscoveryTaskAsync(Subnet subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.Subnets.Include(x => x.List).FirstOrDefaultAsync(x => x.ID == subnet.ID);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {subnet.ID}");

            subnet.List ??= [];
            entity.List ??= [];

            var idDict = entity.List.ToDictionary(x => x.ID, x => x);
            var listId = subnet.List.Select(x => x.ID).ToList();

            foreach (var submitted in subnet.List)
            {
                if (idDict.TryGetValue(submitted.ID, out var existing))
                {
                    if (submitted.Hostname != string.Empty) existing.Hostname = submitted.Hostname;
                    existing.IsMonitoredICMP = submitted.IsMonitoredICMP;
                }
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateIpAsync(IP ip)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.IPs.FirstOrDefaultAsync(x => x.ID == ip.ID);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {ip.ID}");

            entity.Hostname = ip.Hostname;
            entity.IsMonitoredICMP = ip.IsMonitoredICMP;

            if (null != ip.PortsMonitored)
            {
                entity.IsMonitoredTCP = true;
                entity.PortsMonitored = ip.PortsMonitored;
            }

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateSubnetAsync(Subnet subnet)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.Subnets.FirstOrDefaultAsync(x => x.ID == subnet.ID);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {subnet.ID}");

            entity.List = subnet.List;
            entity.Address = subnet.Address;
            entity.StartAddress = subnet.StartAddress;
            entity.EndAddress = subnet.EndAddress;
            entity.SubnetMask = subnet.SubnetMask;

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }
    }
}
