using DashLib.DTO.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using web.Client.Services;

namespace web.Client.Auth
{
    public class RefreshTokenService(IHttpClientFactory httpClientFactory, AuthenticationStateProvider authStateProvider, AuthTokenService store)
    {
        private static readonly SemaphoreSlim _refreshLock = new(1, 1);
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
        private readonly AuthTokenService _tokenStore = store;

        public async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
        {
            await _refreshLock.WaitAsync(cancellationToken);
            try
            {
                var client = _httpClientFactory.CreateClient("NoAuthClient");

                using var res = await client.PostAsync("Auth/v1/refresh", content: null, cancellationToken);
                if (!res.IsSuccessStatusCode)
                    return false;

                var dto = await res.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
                if (dto?.AccessToken.Token is null || dto.AccessToken.Token.Length == 0)
                    return false;

                await _tokenStore.SetTokenAsync(dto.AccessToken.Token);

                if (_authStateProvider is JwtAuthStateProvider jwtProv)
                    jwtProv.NotifyAuthStateChanged();

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _refreshLock.Release();
            }
        }
    }
}
