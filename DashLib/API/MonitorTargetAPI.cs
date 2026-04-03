using DashLib.Interfaces.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DashLib.DankAPI
{
    public class MonitorTargetAPI : IMonitorTargetAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _base;
        private readonly string _baseSubnet;
        private readonly string _baseIp;
        private readonly string _baseDnsCont;
        private readonly string _baseDns;

        public MonitorTargetAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _base = $"{httpClient.BaseAddress}monitortarget/v2";
            _baseSubnet = $"{_base}/subnet";
            _baseIp = $"{_base}/ip";
            _baseDnsCont = $"{_base}/dnscont";
            _baseDns = $"{_base}/dns";
        }

        public async Task<bool> AddDnsContainerAsync(DnsContainer container)
        {
            string endpoint = $"{_baseDnsCont}/post/new";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, container);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> AddDnsTargetAsync(DnsMonitoringTarget dns)
        {
            string endpoint = $"{_baseDns}/post/new";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, dns);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> AddIpTargetAsync(IpMonitoringTarget ip)
        {
            string endpoint = $"{_baseIp}/post/new";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, ip);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> AddSubnetContainerAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseSubnet}/post/new";
            var result = await RequestHandler.PostJsonAsync(_httpClient, endpoint, subnet);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteDnsContainerAsync(DnsContainer container)
        {
            string endpoint = $"{_baseDnsCont}/delete";
            var result = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, container);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteDnsContainerByIdAsync(int id)
        {
            string endpoint = $"{_baseDnsCont}/delete/byid?id={id}";
            var result = await RequestHandler.DeleteAsync(_httpClient, endpoint);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteDnsTargetAsync(DnsMonitoringTarget dns)
        {
            string endpoint = $"{_baseDns}/delete";
            var result = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, dns);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteIpTargetAsync(IpMonitoringTarget ip)
        {
            string endpoint = $"{_baseIp}/delete";
            var result = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, ip);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSubnetByIdAsync(int id)
        {
            string endpoint = $"{_baseSubnet}/delete/byid?id={id}";
            var result = await RequestHandler.DeleteAsync(_httpClient, endpoint);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> DeleteSubnetContainerAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseSubnet}/delete";
            var result = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, subnet);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<List<DnsContainer>> GetAllDnsContainersAsync()
        {
            string endpoint = $"{_baseDnsCont}/get/all";
            var list = await RequestHandler.GetFromJsonAsync<List<DnsContainer>>(_httpClient, endpoint);
            return list ?? [];
        }

        public async Task<List<DnsMonitoringTarget>> GetAllDnsTargetsAsync()
        {
            string endpoint = $"{_baseDns}/get/all";
            var list = await RequestHandler.GetFromJsonAsync<List<DnsMonitoringTarget>>(_httpClient, endpoint);
            return list ?? [];
        }

        public async Task<List<IpMonitoringTarget>> GetAllIpsAsync()
        {
            string endpoint = $"{_baseIp}/get/all";
            var list = await RequestHandler.GetFromJsonAsync<List<IpMonitoringTarget>>(_httpClient, endpoint);
            return list ?? [];
        }

        public async Task<List<SubnetContainer>> GetAllSubnetContainersAsync()
        {
            string endpoint = $"{_baseSubnet}/get/all";
            var list = await RequestHandler.GetFromJsonAsync<List<SubnetContainer>>(_httpClient, endpoint);
            return list ?? [];
        }

        public async Task<DnsContainer> GetDnsContainerByIdAsync(int id)
        {
            string endpoint = $"{_baseDnsCont}/get/byid?id={id}";
            var target = await RequestHandler.GetFromJsonAsync<DnsContainer>(_httpClient, endpoint);

            if (target == null) throw new InvalidDataException("No response from API.");
            return target;
        }

        public async Task<DnsMonitoringTarget> GetDnsTargetByIdAsync(int id)
        {
            string endpoint = $"{_baseDns}/get/byid?id={id}";
            var target = await RequestHandler.GetFromJsonAsync<DnsMonitoringTarget>(_httpClient, endpoint);

            if (target == null) throw new InvalidDataException("No response from API.");
            return target;
        }

        public async Task<IpMonitoringTarget> GetIpTargetByIdAsync(int id)
        {
            string endpoint = $"{_baseIp}/get/byid?id={id}";
            var target = await RequestHandler.GetFromJsonAsync<IpMonitoringTarget>(_httpClient, endpoint);

            if (target == null) throw new InvalidDataException("No response from API.");
            return target;
        }

        public async Task<SubnetContainer> GetSubnetContainerByIdAsync(int id)
        {
            string endpoint = $"{_baseSubnet}/get/byid?id={id}";
            var target = await RequestHandler.GetFromJsonAsync<SubnetContainer>(_httpClient, endpoint);

            if (target == null) throw new InvalidDataException("No response from API.");
            return target;
        }

        public async Task<bool> RunDiscoveryTaskAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_base}/startdiscovery";
            await RequestHandler.PostJsonAsync(_httpClient, endpoint, subnet);
            return true;
        }

        public async Task<bool> UpdateDnsContainerAsync(DnsContainer container)
        {
            string endpoint = $"{_baseDnsCont}/update";
            var result = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, container);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> UpdateDnsTargetAsync(DnsMonitoringTarget dns)
        {
            string endpoint = $"{_baseDns}/update";
            var result = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, dns);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> UpdateIpTargetAsync(IpMonitoringTarget ip)
        {
            string endpoint = $"{_baseIp}/update";
            var result = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, ip);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }

        public async Task<bool> UpdateSubnetContainerAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseSubnet}/update";
            var result = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, subnet);

            if (result == null) return false;
            if (result.IsSuccessStatusCode) return true;
            return false;
        }
    }
}
