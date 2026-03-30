using DashLib.Models.Network;

namespace DashLib.Interfaces.Network
{
    public interface ISubnetsAPI
    {
        public Task<bool> RunDiscoveryTaskAsync(Subnet subnet);
        public Task<bool> AddSubnetByObjectAsync(Subnet subnet);
        public Task<bool> UpdateSubnetByObjectAsync(Subnet subnet);
        public Task<List<IpMonitoringTarget>> GetAllIpsAsync();
        public Task<IpMonitoringTarget> GetIpByIdAsync(int ID);
        public Task<List<Subnet>> GetAllAsync();
        public Task<bool> DeleteSubnetByObjectAsync(Subnet subnet);
        public Task<bool> EditIpAsync(IpMonitoringTarget ip);
        public Task<bool> DeleteSubnetAsync(int ID);
        public Task<Subnet> GetSubnetByIdAsync(int ID);
        public Task<bool> DeleteIpByObjectAsync(IpMonitoringTarget ip);
        public Task<bool> ReplaceAllSubnetsAsync(List<Subnet> subnets);
    }
}