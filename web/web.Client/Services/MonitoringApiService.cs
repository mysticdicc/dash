using DashLib.DankAPI;
using DashLib.Interfaces;
using DashLib.Monitoring;
using DashLib.Network;

namespace web.Client.Services
{
    public class MonitoringApiService(MonitoringAPI monitoringApi, NotificationService notificationService) : IMonitoringAPI
    {
        private readonly MonitoringAPI _monitoringApi = monitoringApi;
        private readonly NotificationService _notificationService = notificationService;

        async public Task<List<IP>> GetAllPollsAsync()
        {
            try
            {
                var response = await _monitoringApi.GetAllPollsAsync();

                if (response == null || response.Count == 0)
                {
                    _notificationService.ShowAsync("No Data", "No monitored device polls were returned.");
                    return [];
                }

                bool anyStates = response.Any(ip => ip.MonitorStateList != null && ip.MonitorStateList.Count() > 0);
                if (!anyStates)
                {
                    _notificationService.ShowAsync("No Data", "No monitor states found for any device.");
                    return response;
                }

                _notificationService.ShowAsync("Success", "Retrieved all monitored device polls");
                return response;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to retrieve device polls", ex.Message);
                return [];
            }
            
        }

        async public Task<List<IP>> GetMonitoredIpsAsync()
        {
            try
            {
                var response = await _monitoringApi.GetMonitoredIpsAsync();
                _notificationService.ShowAsync("Success", "Retrieved all monitored IPs");
                return response;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to retrieve monitored IPs", ex.Message);
                return [];
            }
            
        }

        async public Task<List<MonitorState>> GetByDeviceByIdAsync(int ID)
        {
            try
            {
                var response = await _monitoringApi.GetByDeviceByIdAsync(ID);
                _notificationService.ShowAsync("Success", $"Retrieved states for device {ID}");
                return response;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to retrieve device by ID", ex.Message);
                return [];
            }
            
        }

        async public Task<bool> RestartServiceAsync()
        {
            try
            {
                await _monitoringApi.RestartServiceAsync();
                _notificationService.ShowAsync("Success", "Monitoring service has been restarted successfully");
                return true;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failure restarting monitoring service", ex.Message);
                return false;
            }
        }
    }
}
