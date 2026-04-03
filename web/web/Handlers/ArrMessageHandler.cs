using System.Net.Http.Headers;
using web.Services;

namespace web.Handlers
{
    public class ArrMessageHandler : DelegatingHandler
    {
        public const string ApiKeyOptionKey = "ArrApiKey";

        public ArrMessageHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Options.TryGetValue(new HttpRequestOptionsKey<string>(ApiKeyOptionKey), out var apiKey)
                && !string.IsNullOrEmpty(apiKey))
            {
                request.Headers.Add("X-Api-Key", apiKey);
            }
            else
            {
                throw new InvalidOperationException("Arr API key not found in request options");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}