using DashLib.DTO;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitoringAPI
    {
        public Task<List<PortState>> GetAllPortStatesAsync();
        public Task<List<PingState>> GetAllPingStatesAsync();
        public Task<List<IpMonitoringTarget>> GetAllMonitoredIpsAsync();
        public Task<List<DnsMonitoringTarget>> GetAllMonitoredDnsAsync();
        public Task<IpMonitoringTarget> GetIpMonitorStatesByDeviceIdAsync(int id);
        public Task<DnsMonitoringTarget> GetDnsMonitorStatesByDeviceIdAsync(int id);
        public Task<bool> RestartServiceAsync();
        public Task<IpMonitoringTarget> GetIpMonitoringTargetByStringAddressAsync(string ip);
        public Task<DnsMonitoringTarget> GetDnsMonitoringTargetByStringAddressAsync(string address);
        public Task<PingResponseDto> PingDeviceByStringIpAsync(string ip);
        public Task<bool> PostDnsPollsAsync(List<DnsMonitoringTarget> dnsList);
        public Task<bool> PostIpPollsAsync(List<IpMonitoringTarget> ipList);
    }
}
