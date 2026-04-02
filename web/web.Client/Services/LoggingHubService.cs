using DashLib.API;
using DashLib.Interfaces.Logging;
using DashLib.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;

namespace web.Client.Services
{
    public class LoggingHubService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly ILoggingAPI _loggingApi;
        private readonly AuthTokenService _tokenStore;
        private string _baseAddr;
        public List<LogEntry> Logs { get; private set; } = [];
        public event Action? OnLogsUpdated;

        public LoggingHubService(ILoggingAPI loggingApi, string baseAddr, AuthTokenService tokenStore)
        {
            _loggingApi = loggingApi;
            _tokenStore = tokenStore;
            _baseAddr = baseAddr.TrimEnd('/') + "/loghub";
            _ = StartAsync(_baseAddr);
        }

        public async Task StartAsync(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.AccessTokenProvider = async () => await _tokenStore.GetTokenAsync();
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<LogEntry>("ReceiveLog", (log) =>
            {
                Logs.Insert(0, log);
                OnLogsUpdated?.Invoke();
            });

            _hubConnection.On<List<LogEntry>>("ReceiveLogs", (logs) =>
            {
                Logs.InsertRange(0, logs);
                OnLogsUpdated?.Invoke();
            });

            await _hubConnection.StartAsync();
        }

        public async Task LoadInitialLogsAsync()
        {
            var logs = await _loggingApi.GetAllLogEntriesAsync();
            if (logs != null)
            {
                Logs = logs.OrderByDescending(x => x.Timestamp).ToList();
                OnLogsUpdated?.Invoke();
            }
        }

        public void ClearLogs()
        {
            Logs.Clear();
            OnLogsUpdated?.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
