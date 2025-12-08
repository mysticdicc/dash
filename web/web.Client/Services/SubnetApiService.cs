using danklibrary.DankAPI;
using danklibrary.Network;
using web.Client.Components;

namespace web.Client.Services
{
    public class SubnetApiService(SubnetsAPI subnetsAPI, NotificationService notificationService) : ISubnetsAPI
    {
        private readonly SubnetsAPI _subnetsAPI = subnetsAPI;
        private readonly NotificationService _notificationService = notificationService;

        public async Task<bool> RunDiscoveryTaskAsync(Subnet subnet)
        {
            try
            {
                await _subnetsAPI.RunDiscoveryTaskAsync(subnet);
                _notificationService.ShowAsync("Success", "Discovery completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to run discovery", ex.Message);
                throw;
            }
        }

        public async Task<bool> AddSubnetByObjectAsync(Subnet subnet)
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

        public async Task<bool> UpdateSubnetByObjectAsync(Subnet subnet)
        {
            try
            {
                await _subnetsAPI.UpdateSubnetByObjectAsync(subnet);
                _notificationService.ShowAsync("Updated subnet", $"Subnet {IP.ConvertToString(subnet.Address)} has been updated.");

                return true;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync("Failed to update object", ex.Message);
                return false;
            }
        }

        public async Task<List<Subnet>> GetAllAsync()
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

        public async Task<bool> DeleteSubnetByObjectAsync(Subnet subnet)
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

        public async Task<bool> EditIpAsync(IP ip)
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

        public async Task<bool> DiscoveryUpdateAsync(Subnet subnet)
        {
            try
            {
                await _subnetsAPI.DiscoveryUpdateAsync(subnet);
                _notificationService.ShowAsync("Success", "Discovery results updated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to update discovery results", ex.Message);

                return false;
            }
        }

        public async Task<Subnet> GetSubnetByIdAsync(int ID)
        {
            try
            {
                var subnet = await _subnetsAPI.GetSubnetByIdAsync(ID);
                return subnet;
            }
            catch(Exception ex)
            {
                _notificationService.ShowAsync($"Failed to fetch subnet with ID: {ID}", ex.Message);
                return new Subnet();
            }
        }
    }
}