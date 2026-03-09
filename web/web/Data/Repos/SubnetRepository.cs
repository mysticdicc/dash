using dankweb.API;
using DashLib.Interfaces.Network;
using DashLib.Network;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class SubnetRepository(IDbContextFactory<DashDbContext> dbContext) : ISubnetRepository
    {
        private readonly IDbContextFactory<DashDbContext> _DbFactory = dbContext;

        public Task<bool> AddNewIpAsync(IP ip)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddSubnetAsync(Subnet subnet)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteIpAsync(IP ip)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSubnetAsync(Subnet subnet)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IP>> GetAllIpsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Subnet>> GetAllSubnetsWithIpsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IP> GetIpByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Subnet> GetSubnetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RunDeviceDiscoveryUpdateAsync(Subnet subnet)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateIpAsync(IP ip)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSubnetAsync(Subnet subnet)
        {
            throw new NotImplementedException();
        }
    }
}
