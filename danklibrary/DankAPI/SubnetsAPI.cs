using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using danklibrary.Interfaces;
using danklibrary.Network;

namespace danklibrary.DankAPI
{
    public class SubnetsAPI : ISubnetsAPI
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseEndpoint;

        public SubnetsAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseEndpoint = $"{httpClient.BaseAddress}subnets/v2";
        }

        public async Task<bool> RunDiscoveryTaskAsync(Subnet subnet)
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

        public async Task<bool> AddSubnetByObjectAsync(Subnet subnet)
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

        public async Task<bool> UpdateSubnetByObjectAsync(Subnet subnet)
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

        public async Task<List<Subnet>> GetAllAsync()
        {
            string endpoint = $"{_baseEndpoint}/get/all";
            try
            {
                var result = await RequestHandler.GetFromJsonAsync<List<Subnet>>(_httpClient, endpoint);
                if (result is null)
                    throw new InvalidOperationException("No subnets found.");
                return result;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteSubnetByObjectAsync(Subnet subnet)
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

        public async Task<bool> EditIpAsync(IP ip)
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

        public async Task<bool> DiscoveryUpdateAsync(Subnet subnet)
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

        public async Task<Subnet> GetSubnetByIdAsync(int ID)
        {
            string endpoint = $"{_baseEndpoint}/get/byid?id={ID}";

            try
            {
                var response = await RequestHandler.GetFromJsonAsync<Subnet>(_httpClient, endpoint);
                if (response == null)
                    throw new InvalidOperationException("Failed to receive subnet from endpoint.");
                return response;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteIpByObjectAsync(IP ip)
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
    }
}
