using Microsoft.AspNetCore.Components;

namespace web.Client.Services
{
    public class NotificationService
    {
        private Func<string, string, Task>? _showNotificationCallback;

        public void RegisterNotification(Func<string, string, Task> callback)
        {
            _showNotificationCallback = callback;
        }

        public async void ShowAsync(string title, string message)
        {
            if (_showNotificationCallback is not null)
            {
                await _showNotificationCallback(title, message);
            }
        }
    }
}