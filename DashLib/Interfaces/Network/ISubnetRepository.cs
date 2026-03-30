using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Network
{
    public interface ISubnetRepository
    {
        public Task<bool> SubmitDiscoveryTaskAsync(Subnet subnet);
        public Task<bool> AddSubnetAsync(Subnet subnet);
        public Task<bool> UpdateSubnetAsync(Subnet subnet);
        public Task<IReadOnlyList<IpMonitoringTarget>> GetAllIpsAsync();
        public Task<IpMonitoringTarget> GetIpByIdAsync(int id);
        public Task<IReadOnlyList<Subnet>> GetAllSubnetsWithIpsAsync();
        public Task<bool> DeleteSubnetAsync(Subnet subnet);
        public Task<bool> AddNewIpAsync(IpMonitoringTarget ip);
        public Task<bool> UpdateIpAsync(IpMonitoringTarget ip);
        public Task<Subnet> GetSubnetByIdAsync(int id);
        public Task<bool> DeleteIpAsync(IpMonitoringTarget ip);
    }
}
