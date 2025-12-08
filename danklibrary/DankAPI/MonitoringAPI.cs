using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using danklibrary.Monitoring;
using danklibrary.Network;

namespace danklibrary.DankAPI
{
    public class MonitoringAPI : IMonitoringAPI
    {
        private readonly HttpClient _httpClient;
        public readonly string _base;

        public MonitoringAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _base = $"{httpClient.BaseAddress}monitoring";
        }

        public async Task<List<IP>> GetAllPollsAsync()
        {
            string endpoint = $"{_base}/v2/get/devicesandstatus";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<IP>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No monitor states returned.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<IP>> GetMonitoredIpsAsync()
        {
            string endpoint = $"{_base}/v2/get/all";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<IP>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No monitored IPs returned.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<MonitorState>> GetByDeviceByIdAsync(int ID)
        {
            string endpoint = $"{_base}/v2/get/byid?ID={ID}";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<MonitorState>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No monitor states returned for device.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateTimerAsync(int monitorDelay)
        {
            string endpoint = $"{_base}/post/newtimer";
            try
            {
                var response = await RequestHandler.PostJsonAsync(_httpClient, endpoint, monitorDelay);
                if (response is null)
                    throw new InvalidOperationException("No response from timer update.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> GetTimerAsync()
        {
            string endpoint = $"{_base}/get/currenttimer";
            try
            {
                var response = await RequestHandler.GetFromJsonAsync<HttpResponseMessage>(_httpClient, endpoint);
                if (response is null)
                    throw new InvalidOperationException("No response from timer query.");
                return int.Parse(await response.Content.ReadAsStringAsync());
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> RestartServiceAsync()
        {
            string endpoint = $"{_base}/v2/service/restart";
            try
            {
                var response = await RequestHandler.GetFromJsonAsync<HttpResponseMessage>(_httpClient, endpoint);
                if (response is null)
                    throw new InvalidOperationException("No response from restart service.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> PostNewDevicePollAsync(List<IP> ips)
        {
            string endpoint = $"{_base}/v2/new/polls";

            try
            {
                await RequestHandler.PostJsonAsync(_httpClient, endpoint, ips);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
