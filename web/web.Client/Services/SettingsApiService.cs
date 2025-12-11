using DashLib.DankAPI;
using DashLib.Interfaces;
using DashLib.Settings;

namespace web.Client.Services
{
    public class SettingsApiService(NotificationService notification, SettingsAPI settingsAPI) : ISettingsAPI
    {
        private readonly NotificationService _notification = notification;
        private readonly SettingsAPI _settingsAPI = settingsAPI;
        public AllSettings? _currentSettings;

        public async Task<bool> CreateNewSettingsAsync(AllSettings newSettings)
        {
            try
            {
                await _settingsAPI.CreateNewSettingsAsync(newSettings);
                _notification.ShowAsync("Settings Created", "New settings have been created successfully.");
                _currentSettings = await _settingsAPI.GetCurrentSettingsAsync();
                return true;
            }
            catch(Exception ex)
            {
                _notification.ShowAsync("Failed to create settings", ex.Message);
                return false;
            }
        }

        public async Task<AllSettings> GetCurrentSettingsAsync()
        {
            try
            {
                var response = await _settingsAPI.GetCurrentSettingsAsync();

                if (null == response.MonitoringSettings || null == response.SubnetSettings || null == response.DashboardSettings)
                {
                    _notification.ShowAsync("Settings Corrupted", "Settings object is missing key child objects");
                    return new AllSettings(true);
                }

                _currentSettings = response;
                return response;
            }
            catch (Exception ex)
            {
                _notification.ShowAsync("Failed to get current settings", ex.Message);
                return new AllSettings(true);
            }
        }
    }
}
