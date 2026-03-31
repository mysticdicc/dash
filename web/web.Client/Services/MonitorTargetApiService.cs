using DashLib.DankAPI;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Network;
using System.ComponentModel;
using System.Net;
using web.Client.Components;

namespace web.Client.Services
{
    public class MonitorTargetApiService(MonitorTargetAPI subnetsAPI, NotificationService notificationService) : IMonitorTargetAPI
    {
        private readonly MonitorTargetAPI _subnetsAPI = subnetsAPI;
        private readonly NotificationService _notificationService = notificationService;

        public async Task<bool> AddDnsContainerAsync(DnsContainer container)
        {
            try
            {
                var result = await _subnetsAPI.AddDnsContainerAsync(container);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully added DNS container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error adding DNS container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error adding DNS container", ex.Message);
                return false;
            }
        }

        public async Task<bool> AddDnsTargetAsync(DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _subnetsAPI.AddDnsTargetAsync(dns);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully added DNS target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error adding DNS target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error adding DNS target", ex.Message);
                return false;
            }
        }

        public async Task<bool> AddIpTargetAsync(IpMonitoringTarget ip)
        {
            try
            {
                var result = await _subnetsAPI.AddIpTargetAsync(ip);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully added IP target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error adding IP target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error adding IP target", ex.Message);
                return false;
            }
        }

        public async Task<bool> AddSubnetContainerAsync(SubnetContainer subnet)
        {
            try
            {
                var result = await _subnetsAPI.AddSubnetContainerAsync(subnet);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully added subnet container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error adding subnet container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error adding subnet container", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteDnsContainerAsync(DnsContainer container)
        {
            try
            {
                var result = await _subnetsAPI.DeleteDnsContainerAsync(container);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted DNS container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting DNS container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting DNS container", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteDnsContainerByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.DeleteDnsContainerByIdAsync(id);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted DNS container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting DNS container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting DNS container", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteDnsTargetAsync(DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _subnetsAPI.DeleteDnsTargetAsync(dns);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted DNS target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting DNS target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting DNS target", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteIpTargetAsync(IpMonitoringTarget ip)
        {
            try
            {
                var result = await _subnetsAPI.DeleteIpTargetAsync(ip);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted IP target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting IP target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting IP target", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteSubnetByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.DeleteSubnetByIdAsync(id);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted subnet container by ID.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting subnet container by ID", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting subnet container by ID", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteSubnetContainerAsync(SubnetContainer subnet)
        {
            try
            {
                var result = await _subnetsAPI.DeleteSubnetContainerAsync(subnet);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully deleted subnet container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error deleting subnet container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error deleting subnet container", ex.Message);
                return false;
            }
        }

        public async Task<List<DnsContainer>> GetAllDnsContainersAsync()
        {
            try
            {
                var result = await _subnetsAPI.GetAllDnsContainersAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching DNS containers", ex.Message);
                return [];
            }
        }

        public async Task<List<DnsMonitoringTarget>> GetAllDnsTargetsAsync()
        {
            try
            {
                var result = await _subnetsAPI.GetAllDnsTargetsAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching DNS targets", ex.Message);
                return [];
            }
        }

        public async Task<List<IpMonitoringTarget>> GetAllIpsAsync()
        {
            try
            {
                var result = await _subnetsAPI.GetAllIpsAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching IP targets", ex.Message);
                return [];
            }
        }

        public async Task<List<SubnetContainer>> GetAllSubnetContainersAsync()
        {
            try
            {
                var result = await _subnetsAPI.GetAllSubnetContainersAsync();
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching subnet containers", ex.Message);
                return [];
            }
        }

        public async Task<DnsContainer> GetDnsContainerByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.GetDnsContainerByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching subnet container by ID", ex.Message);
                return new DnsContainer("");
            }
        }

        public async Task<DnsMonitoringTarget> GetDnsTargetByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.GetDnsTargetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching DNS target by ID", ex.Message);
                return new DnsMonitoringTarget();
            }
        }

        public async Task<IpMonitoringTarget> GetIpTargetByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.GetIpTargetByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching IP target by ID", ex.Message);
                return new IpMonitoringTarget();
            }
        }

        public async Task<SubnetContainer> GetSubnetContainerByIdAsync(int id)
        {
            try
            {
                var result = await _subnetsAPI.GetSubnetContainerByIdAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error fetching subnet container by ID", ex.Message);
                return new SubnetContainer();
            }
        }

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
                return false;
            }
        }

        public async Task<bool> UpdateDnsContainerAsync(DnsContainer container)
        {
            try
            {
                var result = await _subnetsAPI.UpdateDnsContainerAsync(container);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully updated DNS container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error updating DNS container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error updating DNS container", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateDnsTargetAsync(DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _subnetsAPI.UpdateDnsTargetAsync(dns);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully updated DNS target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error updating DNS target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error updating DNS target", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateIpTargetAsync(IpMonitoringTarget ip)
        {
            try
            {
                var result = await _subnetsAPI.UpdateIpTargetAsync(ip);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully updated IP target.");
                }
                else
                {
                    _notificationService.ShowAsync("Error updating IP target", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error updating IP target", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateSubnetContainerAsync(SubnetContainer subnet)
        {
            try
            {
                var result = await _subnetsAPI.UpdateSubnetContainerAsync(subnet);

                if (result)
                {
                    _notificationService.ShowAsync("Success", "Successfully updated subnet container.");
                }
                else
                {
                    _notificationService.ShowAsync("Error updating subnet container", "No exception was generated but no change was detected by API.");
                }

                return result;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error updating subnet container", ex.Message);
                return false;
            }
        }
    }
}