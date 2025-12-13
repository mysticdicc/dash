using DashLib.Network;

namespace DashLib.Interfaces
{
    public interface ISubnetsAPI
    {
        public Task<bool> RunDiscoveryTaskAsync(Subnet subnet);
        public Task<bool> AddSubnetByObjectAsync(Subnet subnet);
        public Task<bool> UpdateSubnetByObjectAsync(Subnet subnet);
        public Task<List<IP>> GetAllIpsAsync();
        public Task<IP> GetIpByIdAsync(int ID);
        public Task<List<Subnet>> GetAllAsync();
        public Task<bool> DeleteSubnetByObjectAsync(Subnet subnet);
        public Task<bool> EditIpAsync(IP ip);
        public Task<bool> DeleteSubnetAsync(int ID);
        public Task<bool> DiscoveryUpdateAsync(Subnet subnet);
        public Task<Subnet> GetSubnetByIdAsync(int ID);
        public Task<bool> DeleteIpByObjectAsync(IP ip);
        public Task<bool> ReplaceAllSubnetsAsync(List<Subnet> subnets);
    }
}