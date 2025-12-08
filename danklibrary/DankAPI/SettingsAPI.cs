using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using danklibrary.Settings;

namespace danklibrary.DankAPI
{
    public class SettingsAPI(HttpClient httpClient) : ISettingsAPI
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _base = $"{httpClient.BaseAddress}settings/v2";

        public async Task<bool> CreateNewSettingsAsync(AllSettings newSettings)
        {
            string endpoint = $"{_base}/post/new";

            try
            {
                await RequestHandler.PostJsonAsync(_httpClient, endpoint, newSettings);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public Task<AllSettings> GetCurrentSettingsAsync()
        {
            string endpoint = $"{_base}/get/current";

            try
            {
                var response = RequestHandler.GetFromJsonAsync<AllSettings>(_httpClient, endpoint);

                if (null == response)
                    throw new HttpRequestException("Failed to retrieve current settings.");

                return response!;
            }
            catch
            {
                throw;
            }
        }
    }
}
