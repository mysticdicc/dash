using danklibrary.Monitoring;
using danklibrary.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danklibrary.DankAPI
{
    public interface IMonitoringAPI
    {
        public Task<List<IP>> GetAllPollsAsync();
        public Task<List<IP>> GetMonitoredIpsAsync();
        public Task<List<MonitorState>> GetByDeviceByIdAsync(int ID);
        public Task<bool> RestartServiceAsync();
    }
}
