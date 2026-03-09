using dankweb.API;
using DashLib.Interfaces.Monitoring;
using DashLib.Monitoring;
using DashLib.Network;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class MonitoringRepository(IDbContextFactory<DashDbContext> dbContext) : IMonitoringRepository
    {
        private readonly IDbContextFactory<DashDbContext> _DbFactory = dbContext;

        public Task<IReadOnlyList<IP>> GetMonitoredDevicesAndStatusAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IP>> GetAllMonitoredDevicesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<MonitorState>> GetMonitorStatesByDeviceIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMonitoringRepository.AddMonitorStatesFromListIpAsync(List<IP> ips)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<MonitorState>> IMonitoringRepository.GetAllMonitorStatesAsync()
        {
            throw new NotImplementedException();
        }
    }
}