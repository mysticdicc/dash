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
        public Task<List<IP>> GetAllPollsAsync();
        public Task<List<IP>> GetMonitoredIpsAsync();
        public Task<List<MonitorState>> GetByDeviceByIdAsync(int ID);
        public Task<bool> RestartServiceAsync();
        public Task<IP> GetDeviceAndMonitorStatesByStringIpAsync(string ip);
        public Task<PingResponseDto> PingDeviceByStringIpAsync(string ip);
    }
}
