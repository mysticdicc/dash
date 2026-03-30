using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitorTargetRepository
    {
        public Task<bool> SubmitDiscoveryTaskAsync(SubnetContainer subnet);
        public Task<bool> AddSubnetContainerAsync(SubnetContainer subnet);
        public Task<bool> AddIpTargetAsync(IpMonitoringTarget ip);
        public Task<bool> AddDnsContainerAsync(DnsContainer container);
        public Task<bool> AddDnsTargetAsync(DnsMonitoringTarget dns);
        public Task<bool> UpdateSubnetContainerAsync(SubnetContainer subnet);
        public Task<bool> UpdateDnsContainerAsync(DnsContainer container);
        public Task<bool> UpdateIpTargetAsync(IpMonitoringTarget ip);
        public Task<bool> UpdateDnsTargetAsync(DnsMonitoringTarget dns);
        public Task<IReadOnlyList<IpMonitoringTarget>> GetAllIpsAsync();
        public Task<IReadOnlyList<DnsMonitoringTarget>> GetAllDnsTargetsAsync();
        public Task<IReadOnlyList<SubnetContainer>> GetAllSubnetContainersAsync();
        public Task<IReadOnlyList<DnsContainer>> GetAllDnsContainersAsync();
        public Task<IpMonitoringTarget> GetIpTargetByIdAsync(int id);
        public Task<DnsContainer> GetDnsContainerByIdAsync(int id);
        public Task<SubnetContainer> GetSubnetContainerByIdAsync(int id);
        public Task<DnsMonitoringTarget> GetDnsTargetByIdAsync(int id);
        public Task<bool> DeleteSubnetContainerAsync(SubnetContainer subnet);
        public Task<bool> DeleteDnsContainerAsync(DnsContainer container);
        public Task<bool> DeleteIpTargetAsync(IpMonitoringTarget ip);
        public Task<bool> DeleteDnsTargetAsync(DnsMonitoringTarget dns);
        public Task<bool> DeleteSubnetByIdAsync(int id);
        public Task<bool> DeleteDnsContainerByIdAsync(int id);
    }
}
