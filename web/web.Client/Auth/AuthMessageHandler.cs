using System.Net;
using System.Net.Http.Headers;
using web.Client.Services;

namespace web.Client.Auth
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly AuthTokenService _tokenService;
        private readonly RefreshTokenService _refreshService;

        public AuthMessageHandler(AuthTokenService tokenService, RefreshTokenService refreshService)
        {
            _tokenService = tokenService;
            _refreshService = refreshService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsAuthEndpoint(request.RequestUri))
                return await base.SendAsync(request, cancellationToken);

            var retryRequest = await CloneHttpRequestMessageAsync(request, cancellationToken);

            await AddBearerIfPresentAsync(request);
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
                return response;

            var refreshed = await _refreshService.TryRefreshAsync(cancellationToken);
            if (!refreshed)
                return response;

            response.Dispose();

            await AddBearerIfPresentAsync(retryRequest);
            return await base.SendAsync(retryRequest, cancellationToken);
        }

        private async Task AddBearerIfPresentAsync(HttpRequestMessage request)
        {
            var token = await _tokenService.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private static bool IsAuthEndpoint(Uri? uri)
        {
            var path = uri?.AbsolutePath ?? string.Empty;

            return path.Contains("/Auth/v1/login", StringComparison.OrdinalIgnoreCase) ||
                   path.Contains("/Auth/v1/register", StringComparison.OrdinalIgnoreCase) ||
                   path.Contains("/Auth/v1/refresh", StringComparison.OrdinalIgnoreCase) ||
                   path.Contains("/Auth/v1/logout", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version,
                VersionPolicy = request.VersionPolicy
            };

            foreach (var header in request.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            if (request.Content is not null)
            {
                var bytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                clone.Content = new ByteArrayContent(bytes);

                foreach (var header in request.Content.Headers)
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }
}
