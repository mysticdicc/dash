using danklibrary.DankAPI;
using danklibrary.Dashboard;
using danklibrary.Interfaces;
using System.Collections.Generic;

namespace web.Client.Services
{
    public class DashboardApiService : IDashAPI
    {
        private readonly DashAPI _dashAPI;
        private readonly NotificationService _notificationService;

        public DashboardApiService(DashAPI dashAPI, NotificationService notificationService)
        {
            _dashAPI = dashAPI;
            _notificationService = notificationService;
        }

        public async Task<List<DashboardItemBase>> GetAllItemsAsync()
        {
            try
            {
                var list = await _dashAPI.GetAllItemsAsync();

                foreach (var item in list)
                {
                    if (item is DirectoryItem folder && folder.Children.Count > 0)
                    {
                        foreach (var child in folder.Children)
                        {
                            child.Parent = folder;
                        }
                    }
                }

                return list;
            }
            catch (InvalidOperationException)
            {
                 _notificationService.ShowAsync("No Dashboard Items", 
                    "No dashboard items found. Create one to get started.");

                return [];
            }
            catch (Exception ex)
            {
                 _notificationService.ShowAsync("Error Loading Dashboard", 
                    $"Failed to load dashboard items: {ex.Message}");

                return [];
            }
        }

        public async Task<bool> SaveItemAsync(DashboardItemBase item)
        {
            try
            {
                await _dashAPI.SaveItemAsync(item);

                _notificationService.ShowAsync("Item Created",
                    $"The dashboard item '{item.DisplayName}' has been created.");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error Saving Item", 
                    $"Failed to save '{item.DisplayName}'. {ex.Message}");

                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(DashboardItemBase item)
        {
            try
            {
                await _dashAPI.DeleteItemAsync(item);
                _notificationService.ShowAsync("Item Deleted", 
                    $"The dashboard item '{item.DisplayName}' has been deleted.");
                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error Deleting Item", 
                    $"Failed to delete '{item.DisplayName}'. {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EditItemAsync(DashboardItemBase item)
        {
            try
            {
                await _dashAPI.EditItemAsync(item);
                _notificationService.ShowAsync("Item Edited", 
                    $"The dashboard item '{item.DisplayName}' has been edited.");

                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Error Editing Item", 
                    $"Failed to edit '{item.DisplayName}'. {ex.Message}");

                return false;
            }
        }   
    }
}