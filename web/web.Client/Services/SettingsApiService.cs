using danklibrary.DankAPI;
using danklibrary.Settings;

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
                _currentSettings = response;
                return response;
            }
            catch (Exception ex)
            {
                _notification.ShowAsync("Failed to create settings", ex.Message);
                return new AllSettings();
            }
        }
    }
}
