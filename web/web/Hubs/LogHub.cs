using DashLib.Models;
using Microsoft.AspNetCore.SignalR;

namespace web.Hubs
{
    public class LogHub : Hub
    {
        public async Task SendLog(LogEntry logEntry)
        {
            await Clients.All.SendAsync("ReceiveLog", logEntry);
        }

        public async Task SendLogs(List<LogEntry> logs)
        {
            await Clients.All.SendAsync("ReceiveLogs", logs);
        }
    }
}
