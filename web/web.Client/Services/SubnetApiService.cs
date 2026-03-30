using DashLib.DankAPI;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Network;
using web.Client.Components;

namespace web.Client.Services
{
    public class SubnetApiService(MonitorTargetAPI subnetsAPI, NotificationService notificationService) : IMonitorTargetAPI
    {
        private readonly MonitorTargetAPI _subnetsAPI = subnetsAPI;
        private readonly NotificationService _notificationService = notificationService;

        public async Task<bool> RunDiscoveryTaskAsync(SubnetContainer subnet)
        {
            try
            {
                _subnetsAPI.RunDiscoveryTaskAsync(subnet);
                _notificationService.ShowAsync("Success", "Discovery task running, please come back in a few minutes to check results.");
                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to run discovery", ex.Message);
                throw;
            }
        }

        public async Task<bool> AddSubnetByObjectAsync(SubnetContainer subnet)
        {
            try
            {
                await _subnetsAPI.AddSubnetByObjectAsync(subnet);
                _notificationService.ShowAsync("Success", "Subnet added successfully");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to add subnet", ex.Message);

                return false;
            }
        }

        public async Task<bool> UpdateSubnetByObjectAsync(SubnetContainer subnet)
        {
            try
            {
                await _subnetsAPI.UpdateSubnetByObjectAsync(subnet);
                _notificationService.ShowAsync("Updated subnet", $"Subnet {IpMonitoringTarget.ConvertToString(subnet.Address)} has been updated.");

                return true;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to update object", ex.Message);
                return false;
            }
        }

        public async Task<IpMonitoringTarget> GetIpByIdAsync(int ID)
        {
            try
            {
                return await _subnetsAPI.GetIpByIdAsync(ID);
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync($"Failed to load ip: {ID}", ex.Message);
                return new IP() { Address = new byte[4] };
            }
        }

        public async Task<List<IpMonitoringTarget>> GetAllIpsAsync()
        {
            try
            {
                return await _subnetsAPI.GetAllIpsAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to load ips", ex.Message);
                return [];
            }
        }

        public async Task<List<SubnetContainer>> GetAllAsync()
        {
            try
            {
                return await _subnetsAPI.GetAllAsync();
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to load subnets", ex.Message);
                return [];
            }
        }

        public async Task<bool> DeleteSubnetByObjectAsync(SubnetContainer subnet)
        {
            try
            {
                await _subnetsAPI.DeleteSubnetByObjectAsync(subnet);
                _notificationService.ShowAsync("Success", "Subnet deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to delete subnet", ex.Message);

                return false;
            }
        }

        public async Task<bool> EditIpAsync(IpMonitoringTarget ip)
        {
            try
            {
                await _subnetsAPI.EditIpAsync(ip);
                _notificationService.ShowAsync("Success", "IP address updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to update IP address", ex.Message);

                return false;
            }
        }

        public async Task<bool> DeleteSubnetAsync(int id)
        {
            try
            {
                await _subnetsAPI.DeleteSubnetAsync(id);
                _notificationService.ShowAsync("Success", "Subnet deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to delete subnet", ex.Message);

                return false;
            }
        }

        public async Task<SubnetContainer> GetSubnetByIdAsync(int ID)
        {
            try
            {
                var subnet = await _subnetsAPI.GetSubnetByIdAsync(ID);
                return subnet;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync($"Failed to fetch subnet with ID: {ID}", ex.Message);
                return new SubnetContainer();
            }
        }

        public async Task<bool> DeleteIpByObjectAsync(IpMonitoringTarget ip)
        {
            try
            {
                await _subnetsAPI.DeleteIpByObjectAsync(ip);
                _notificationService.ShowAsync("Success", "IP address deleted successfully");
                return true;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to delete IP", ex.Message);
                return false;
            }
        }

        public async Task<bool> ReplaceAllSubnetsAsync(List<SubnetContainer> subnets)
        {
            try
            {
                await _subnetsAPI.ReplaceAllSubnetsAsync(subnets);
                _notificationService.ShowAsync("Success", "All subnets replaced successfully");
                return true;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to replace all subnets", ex.Message);
                return false;
            }
        }
    }
}