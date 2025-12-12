using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DashLib.DankAPI
{
    public static class RequestHandler
    {
        private const int maxRetries = 3;
        private const int delayMs = 1000;

        public static async Task<HttpResponseMessage> SendWithRetryAsync(
            HttpClient httpClient,
            HttpRequestMessage request,
            CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                response.Dispose();

                await Task.Delay(delayMs, cancellationToken);
            }

            throw new HttpRequestException($"Request failed after {maxRetries} attempts.");
        }

        public static async Task<T?> GetFromJsonAsync<T>(HttpClient httpClient, string endpoint)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(endpoint)
            };

            var response = await SendWithRetryAsync(httpClient, request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<HttpResponseMessage> PostJsonAsync(HttpClient httpClient, string endpoint, object obj)
        {
            var request = new HttpRequestMessage
            {
                Content = JsonContent.Create(obj),
                Method = HttpMethod.Post,
                RequestUri = new Uri(endpoint)
            };

            try
            {
                var response = await SendWithRetryAsync(httpClient, request);
                return response;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<HttpResponseMessage> DeleteAsJsonAsync(HttpClient httpClient, string endpoint, object obj)
        {
            var request = new HttpRequestMessage
            {
                Content = JsonContent.Create(obj),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint)
            };

            try
            {
                var response = await SendWithRetryAsync(httpClient, request);
                return response;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<HttpResponseMessage> DeleteAsync(HttpClient httpClient, string endpoint, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(endpoint)
            };

            var response = await SendWithRetryAsync(httpClient, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"DELETE request to {endpoint} failed with status code {response.StatusCode}");

            return response;
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync(HttpClient httpClient, string endpoint, object obj)
        {
            var request = new HttpRequestMessage
            {
                Content = JsonContent.Create(obj),
                Method = HttpMethod.Put,
                RequestUri = new Uri(endpoint)
            };

            try
            {
                var response = await SendWithRetryAsync(httpClient, request);
                return response;
            }
            catch
            {
                throw;
            }
        }
    }
}
