using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitorStateRepository
    {
        public Task<IReadOnlyList<IpMonitoringTarget>> GetMonitoredIpAndStatusAsync();
        public Task<IReadOnlyList<DnsMonitoringTarget>> GetMonitoredDnsAndStatusAsync();
        public Task<IReadOnlyList<IpMonitoringTarget>> GetAllMonitoredIpsAsync();
        public Task<IReadOnlyList<DnsMonitoringTarget>> GetAllMonitoredDnsAsync();
        public Task<DnsMonitoringTarget> GetDnsMonitorStatesByDeviceIdAsync(int id);
        public Task<IpMonitoringTarget> GetIpMonitorStatesByDeviceIdAsync(int id);
        public Task<bool> AddMonitorStatesFromListIpAsync(List<IpMonitoringTarget> ipList);
        public Task<bool> AddMonitorStatesFromListDnsAsync(List<DnsMonitoringTarget> dnsList);
        public Task<IReadOnlyList<PortState>> GetAllPortStatesAsync();
        public Task<IReadOnlyList<PingState>> GetAllPingStatesAsync();
        public Task<IpMonitoringTarget> GetIpMonitoringTargetByStringAddressAsync(string ip);
        public Task<DnsMonitoringTarget> GetDnsMonitoringTargetByStringAddressAsync(string address);
    }
}
