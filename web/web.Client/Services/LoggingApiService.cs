using DashLib.API;
using DashLib.DankAPI;
using DashLib.Interfaces.Logging;
using DashLib.Models;

namespace web.Client.Services
{
    public class LoggingApiService : ILoggingAPI
    {
        private readonly LoggingAPI _logsAPI;
        private readonly NotificationService _notificationService;

        public LoggingApiService(LoggingAPI logsAPI, NotificationService notificationService)
        {
            _logsAPI = logsAPI;
            _notificationService = notificationService;
        }

        public async Task<List<LogEntry>> GetAllLogEntriesAsync()
        {
            try
            {
                var logs = await _logsAPI.GetAllLogEntriesAsync();
                return logs;
            }
            catch 
            {
                _notificationService.ShowAsync("No Log Entries", "No log entries to display.");
                return [];
            }
        }
    }
}
