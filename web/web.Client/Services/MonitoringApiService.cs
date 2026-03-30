using DashLib.DankAPI;
using DashLib.DTO;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System.Diagnostics;

namespace web.Client.Services
{
    public class MonitoringApiService(MonitorStateAPI monitoringApi, NotificationService notificationService) : IMonitorStatesAPI
    {
        private readonly MonitorStateAPI _monitoringApi = monitoringApi;
        private readonly NotificationService _notificationService = notificationService;

        async public Task<List<IpMonitoringTarget>> GetMonitoredIpsAsync()
        {
            try
            {
                var response = await _monitoringApi.GetMonitoredIpsAsync();
                return response;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to retrieve monitored IPs", ex.Message);
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

        public async Task<IpMonitoringTarget> GetDeviceAndMonitorStatesByStringIpAsync(string ip)
        {
            try
            {
                var result = await _monitoringApi.GetDeviceAndMonitorStatesByStringIpAsync(ip);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return new IpMonitoringTarget();
            }
        }

        public async Task<PingResponseDto> PingDeviceByStringIpAsync(string ip)
        {
            try
            {
                var result = await _monitoringApi.PingDeviceByStringIpAsync(ip);
                return result;
            }
            catch (Exception ex)
            {
                var response = new PingResponseDto()
                {
                    IcmpResponse = false,
                    Exception = ex.Message
                };

                return response;
            }
        }

        public async Task<List<PortState>> GetAllPortStatesAsync()
        {
            try
            {
                var result = await _monitoringApi.GetAllPortStatesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return [];
            }
        }

        public async Task<List<PingState>> GetAllPingStatesAsync()
        {
            try
            {
                var result = await _monitoringApi.GetAllPingStatesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return [];
            }
        }

        public async Task<List<IpMonitoringTarget>> GetAllMonitoredIpsAsync()
        {
            try
            {
                var result = await _monitoringApi.GetAllMonitoredIpsAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return [];
            }
        }

        public async Task<List<DnsMonitoringTarget>> GetAllMonitoredDnsAsync()
        {
            try
            {
                var result = await _monitoringApi.GetAllMonitoredDnsAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return [];
            }
        }

        public async Task<IpMonitoringTarget> GetIpMonitorStatesByDeviceIdAsync(int id)
        {
            try
            {
                var result = await _monitoringApi.GetIpMonitorStatesByDeviceIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return new IpMonitoringTarget();
            }
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitorStatesByDeviceIdAsync(int id)
        {
            try
            {
                var result = await _monitoringApi.GetDnsMonitorStatesByDeviceIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return new DnsMonitoringTarget();
            }
        }

        public async Task<IpMonitoringTarget> GetIpMonitoringTargetByStringAddressAsync(string ip)
        {
            try
            {
                var result = await _monitoringApi.GetIpMonitoringTargetByStringAddressAsync(ip);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return new IpMonitoringTarget();
            }
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitoringTargetByStringAddressAsync(string address)
        {
            try
            {
                var result = await _monitoringApi.GetDnsMonitoringTargetByStringAddressAsync(address);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return new DnsMonitoringTarget();
            }
        }

        public async Task<bool> PostDnsPollsAsync(List<DnsMonitoringTarget> dnsList)
        {
            try
            {
                var result = await _monitoringApi.PostDnsPollsAsync(dnsList);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return false;
            }
        }

        public async Task<bool> PostIpPollsAsync(List<IpMonitoringTarget> ipList)
        {
            try
            {
                var result = await _monitoringApi.PostIpPollsAsync(ipList);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("API failure", ex.Message);
                return false;
            }
        }
    }
}
