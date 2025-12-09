using danklibrary.Network;

namespace danklibrary.DankAPI
{
    public interface ISubnetsAPI
    {
        public Task<bool> RunDiscoveryTaskAsync(Subnet subnet);
        public Task<bool> AddSubnetByObjectAsync(Subnet subnet);
        public Task<bool> UpdateSubnetByObjectAsync(Subnet subnet);
        public Task<List<Subnet>> GetAllAsync();
        public Task<bool> DeleteSubnetByObjectAsync(Subnet subnet);
        public Task<bool> EditIpAsync(IP ip);
        public Task<bool> DeleteSubnetAsync(int ID);
        public Task<bool> DiscoveryUpdateAsync(Subnet subnet);
        public Task<Subnet> GetSubnetByIdAsync(int ID);
        public Task<bool> DeleteIpByObjectAsync(IP ip);
    }
}