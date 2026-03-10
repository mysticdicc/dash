using DashLib.DankAPI;
using DashLib.Interfaces.Logging;
using DashLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.API
{
    public class LoggingAPI : ILoggingAPI
    {
        private readonly HttpClient _httpClient;
        private string _base;

        public LoggingAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _base = $"{_httpClient.BaseAddress}logs";
        }

        public async Task<List<LogEntry>> GetAllLogEntriesAsync()
        {
            var endpoint = $"{_base}/get/all";
            var logs = await RequestHandler.GetFromJsonAsync<List<LogEntry>>(_httpClient, endpoint);
            return logs ?? [];
        }
    }
}
