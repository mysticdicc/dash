using DashLib.Monitoring;
using DashLib.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Interfaces
{
    public interface IMonitoringAPI
    {
        public Task<List<IP>> GetAllPollsAsync();
        public Task<List<IP>> GetMonitoredIpsAsync();
        public Task<List<MonitorState>> GetByDeviceByIdAsync(int ID);
        public Task<bool> RestartServiceAsync();
    }
}
