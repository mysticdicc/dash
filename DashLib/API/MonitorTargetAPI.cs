using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Network;

namespace DashLib.DankAPI
{
    public class MonitorTargetAPI : IMonitorTargetAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseEndpoint;

        public MonitorTargetAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseEndpoint = $"{httpClient.BaseAddress}monitortargets/v2";
        }

        public async Task<bool> RunDiscoveryTaskAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseEndpoint}/startdiscovery";
            try
            {
                await RequestHandler.PostJsonAsync(_httpClient, endpoint, subnet);
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> AddSubnetByObjectAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseEndpoint}/new/byobject";
            try
            {
                var response = await RequestHandler.PostJsonAsync(_httpClient, endpoint, subnet);
                if (response is null)
                    throw new InvalidOperationException("No response from add subnet.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateSubnetByObjectAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseEndpoint}/update/byobject";
            try
            {
                var response = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, subnet);
                if (response is null)
                    throw new InvalidOperationException("No response from update subnet.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IpMonitoringTarget> GetIpByIdAsync(int ID)
        {
            string endpoint = $"{_baseEndpoint}/ips/get/byid?ID={ID}";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<IpMonitoringTarget>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No IPs found.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<IpMonitoringTarget>> GetAllIpsAsync()
        {
            string endpoint = $"{_baseEndpoint}/ips/get/all";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<IpMonitoringTarget>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No IPs found.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<SubnetContainer>> GetAllAsync()
        {
            string endpoint = $"{_baseEndpoint}/get/all";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<SubnetContainer>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No subnets found.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteSubnetByObjectAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseEndpoint}/delete/byobject";
            try
            {
                var response = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, subnet);
                if (response is null)
                    throw new InvalidOperationException("No response from delete subnet.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> EditIpAsync(IpMonitoringTarget ip)
        {
            string endpoint = $"{_baseEndpoint}/ip/put/update";
            try
            {
                var response = await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, ip);
                if (response is null)
                    throw new InvalidOperationException("No response from update IP.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteSubnetAsync(int ID)
        {
            string endpoint = $"{_baseEndpoint}/subnet/delete/byid?ID={ID}";
            try
            {
                var response = await RequestHandler.DeleteAsync(_httpClient, endpoint);
                if (response is null)
                    throw new InvalidOperationException("No response from delete subnet by ID.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DiscoveryUpdateAsync(SubnetContainer subnet)
        {
            string endpoint = $"{_baseEndpoint}/subnet/post/discoveryupdate";
            try
            {
                var response = await RequestHandler.PostJsonAsync(_httpClient, endpoint, subnet);
                if (response is null)
                    throw new InvalidOperationException("No response from discovery update.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<SubnetContainer> GetSubnetByIdAsync(int ID)
        {
            string endpoint = $"{_baseEndpoint}/get/byid?id={ID}";

            try
            {
                var response = await RequestHandler.GetFromJsonAsync<SubnetContainer>(_httpClient, endpoint);
                if (response == null)
                    throw new InvalidOperationException("Failed to receive subnet from endpoint.");
                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteIpByObjectAsync(IpMonitoringTarget ip)
        {
            string endpoint = $"{_baseEndpoint}/ip/delete/byobject";

            try
            {
                var response = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, ip);
                if (response is null)
                    throw new InvalidOperationException("No response from delete IP.");
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ReplaceAllSubnetsAsync(List<SubnetContainer> subnets) 
        {
            List<IpMonitoringTarget> oldIps = [];
            List<SubnetContainer> oldSubnets = [];

            bool success = true;

            try
            {
                oldIps = await GetAllIpsAsync();
            }
            catch { }

            try
            {
                oldSubnets = await GetAllAsync();
            }
            catch { }

            if (oldSubnets.Count > 0)
            {
                foreach (var subnet in oldSubnets)
                {
                    try
                    {
                        await DeleteSubnetByObjectAsync(subnet);
                    } 
                    catch { success = false; }
                }
            }

            if (oldIps.Count > 0)
            {
                foreach (var ip in oldIps)
                {
                    try
                    {
                        await DeleteIpByObjectAsync(ip);
                    } 
                    catch { success = false; }
                }
            }

            foreach (var subnet in subnets)
            {
                try
                {
                    await AddSubnetByObjectAsync(subnet);
                } 
                catch { success = false; }
            }

            return success;
        }
    }
}
