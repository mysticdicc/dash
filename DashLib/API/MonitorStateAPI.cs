using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DashLib.DTO;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;

namespace DashLib.DankAPI
{
    public class MonitorStateAPI : IMonitorStatesAPI
    {
        private readonly HttpClient _httpClient;
        public readonly string _base;

        public MonitorStateAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _base = $"{httpClient.BaseAddress}monitorstate";
        }

        public async Task<List<IpMonitoringTarget>> GetMonitoredIpsAsync()
        {
            string endpoint = $"{_base}/v2/get/all";
            var result = await RequestHandler.GetFromJsonAsync<List<IpMonitoringTarget>>(_httpClient, endpoint);

            if (result is null)
                throw new InvalidOperationException("No monitored IPs returned.");

            return result;
        }

        public async Task<bool> RestartServiceAsync()
        {
            string endpoint = $"{_base}/v2/service/restart";
            var response = await RequestHandler.GetFromJsonAsync<HttpResponseMessage>(_httpClient, endpoint);

            if (response is null)
                throw new InvalidOperationException("No response from restart service.");

            return true;
        }

        public async Task<bool> PostNewDevicePollAsync(List<IpMonitoringTarget> ips)
        {
            string endpoint = $"{_base}/v2/new/polls";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, ips);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<IpMonitoringTarget> GetDeviceAndMonitorStatesByStringIpAsync(string ip)
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/byip?ip={ip}";
            var result = await RequestHandler.GetFromJsonAsync<IpMonitoringTarget>(_httpClient, endpoint);

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

        public async Task<List<PortState>> GetAllPortStatesAsync()
        {
            string endpoint = $"{_base}/v2/get/all/portstates";
            var result = await RequestHandler.GetFromJsonAsync<List<PortState>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<List<PingState>> GetAllPingStatesAsync()
        {
            string endpoint = $"{_base}/v2/get/all/pingstates";
            var result = await RequestHandler.GetFromJsonAsync<List<PingState>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<List<IpMonitoringTarget>> GetAllMonitoredIpsAsync()
        {
            string endpoint = $"{_base}/v2/get/ips";
            var result = await RequestHandler.GetFromJsonAsync<List<IpMonitoringTarget>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<List<DnsMonitoringTarget>> GetAllMonitoredDnsAsync()
        {
            string endpoint = $"{_base}/v2/get/dns";
            var result = await RequestHandler.GetFromJsonAsync<List<DnsMonitoringTarget>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<IpMonitoringTarget> GetIpMonitorStatesByDeviceIdAsync(int id)
        {
            string endpoint = $"{_base}/v2/get/ip/byid?id={id}";
            var result = await RequestHandler.GetFromJsonAsync<IpMonitoringTarget>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitorStatesByDeviceIdAsync(int id)
        {
            string endpoint = $"{_base}/v2/get/dns/byid?id={id}";
            var result = await RequestHandler.GetFromJsonAsync<DnsMonitoringTarget>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<IpMonitoringTarget> GetIpMonitoringTargetByStringAddressAsync(string ip)
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/byip?ip={ip}";
            var result = await RequestHandler.GetFromJsonAsync<IpMonitoringTarget>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<DnsMonitoringTarget> GetDnsMonitoringTargetByStringAddressAsync(string address)
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/byaddress?address={address}";
            var result = await RequestHandler.GetFromJsonAsync<DnsMonitoringTarget>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<bool> PostDnsPollsAsync(List<DnsMonitoringTarget> dnsList)
        {
            string endpoint = $"{_base}/v2/post/dns";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, dnsList);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> PostIpPollsAsync(List<IpMonitoringTarget> ipList)
        {
            string endpoint = $"{_base}/v2/post/ip";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, ipList);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<List<IpMonitoringTarget>> GetMonitoredIpAndStatusAsync()
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/ips";
            var result = await RequestHandler.GetFromJsonAsync<List<IpMonitoringTarget>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }

        public async Task<List<DnsMonitoringTarget>> GetMonitoredDnsAndStatusAsync()
        {
            string endpoint = $"{_base}/v2/get/deviceandstatus/dns";
            var result = await RequestHandler.GetFromJsonAsync<List<DnsMonitoringTarget>>(_httpClient, endpoint);

            if (result == null) throw new InvalidDataException("No response from API");
            return result;
        }
    }
}
