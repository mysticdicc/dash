using DashLib.API;
using DashLib.DankAPI;
using DashLib.Interfaces;

namespace web.Client.Services
{
    public class MailApiService : IMailAPI
    {
        private readonly MailAPI _mailAPI;
        private readonly NotificationService _notificationService;

        public MailApiService(MailAPI mailAPI, NotificationService notificationService)
        {
            _mailAPI = mailAPI;
            _notificationService = notificationService;
        }

        async public Task<bool> SendTestEmailAsync()
        {
            try
            {
                await _mailAPI.SendTestEmailAsync();
                _notificationService.ShowAsync("Success", "Test email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _notificationService.ShowAsync("Failed to send test email", ex.Message);
                return false;
            }
        }
    }
}
