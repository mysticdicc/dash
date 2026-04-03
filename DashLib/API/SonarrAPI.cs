using DashLib.DTO.Arr;
using DashLib.Interfaces.Arr;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using DashLib.Models.MonitoringTargets;

namespace DashLib.API
{
    public class SonarrAPI(IHttpClientFactory factory) : IArrAPI
    {
        private readonly IHttpClientFactory _httpFactory = factory;
        private const string ApiKeyOptionKey = "ArrApiKey";

        public async Task<DiskSizeResponseDto> GetCurrentDiskSpaceAsync(ArrMonitoringTarget target)
        {
            if (string.IsNullOrEmpty(target.ApiKey))
                throw new InvalidOperationException("No API key has been saved for target.");

            string endpoint = $"{target.GetEffectiveAddress()}/api/v3/diskspace";
            var client = _httpFactory.CreateClient("ArrClient");

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Options.Set(new HttpRequestOptionsKey<string>(ApiKeyOptionKey), target.ApiKey);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<DiskSizeResponseDto>();
            if (null == dto)
                throw new InvalidCastException("Unable to deserialize API response.");

            return dto;
        }

        public async Task<HealthResponseDto> GetCurrentHealthStatusAsync(ArrMonitoringTarget target)
        {
            if (string.IsNullOrEmpty(target.ApiKey))
                throw new InvalidOperationException("No API key has been saved for target.");

            string endpoint = $"{target.GetEffectiveAddress()}/api/v3/health";
            var client = _httpFactory.CreateClient("ArrClient");

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Options.Set(new HttpRequestOptionsKey<string>(ApiKeyOptionKey), target.ApiKey);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<HealthResponseDto>();
            if (null == dto)
                throw new InvalidCastException("Unable to deserialize API response.");

            return dto;
        }
    }
}
