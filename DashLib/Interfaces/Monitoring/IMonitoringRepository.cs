using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitoringRepository
    {
        public Task<IReadOnlyList<IP>> GetMonitoredDevicesAndStatusAsync();
        public Task<IReadOnlyList<IP>> GetAllMonitoredDevicesAsync();
        public Task<IReadOnlyList<MonitorState>> GetMonitorStatesByDeviceIdAsync(int ID);
        public Task<bool> AddMonitorStatesFromListIpAsync(List<IP> ips);
        public Task<IReadOnlyList<MonitorState>> GetAllMonitorStatesAsync();
        public Task<IP> GetDeviceAndMonitorStatesByStringIpAsync(string ip);
    }
}
