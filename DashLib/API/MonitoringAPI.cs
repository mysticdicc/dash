using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DashLib.DTO;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;

namespace DashLib.DankAPI
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
            var response = await RequestHandler.GetFromJsonAsync<HttpResponseMessage>(_httpClient, endpoint);

            if (response is null)
                throw new InvalidOperationException("No response from restart service.");

            return true;
        }

        public async Task<bool> PostNewDevicePollAsync(List<IP> ips)
        {
            string endpoint = $"{_base}/v2/new/polls";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, ips);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<IP> GetDeviceAndMonitorStatesByStringIpAsync(string ip)
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/byip?ip={ip}";
            var result = await RequestHandler.GetFromJsonAsync<IP>(_httpClient, endpoint);

            if (result == null) throw new InvalidOperationException("No response from device and monitor state query.");
            return result!;
        }

        public async Task<PingResponseDto> PingDeviceByStringIpAsync(string ip)
        {
            string endpoint = $"{_base}/v2/get/pingstatus?ip={ip}";
            var result = await RequestHandler.GetFromJsonAsync<PingResponseDto>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");

            return result;
        }
    }
}
