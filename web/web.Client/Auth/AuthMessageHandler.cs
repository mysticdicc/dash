using System.Net.Http.Headers;
using web.Client.Services;

namespace web.Client.Auth
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly TokenStorageService _tokenStore;

        public AuthMessageHandler(TokenStorageService tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenStore.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
