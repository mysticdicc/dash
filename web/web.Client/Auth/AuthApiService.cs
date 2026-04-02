using DashLib.DTO.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Text.Json;
using web.Client.Services;

namespace web.Client.Auth
{
    public class AuthApiService
    {
        private readonly HttpClient _http;
        private readonly AuthTokenService _tokenStore;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthApiService(HttpClient http, AuthTokenService tokenStore, AuthenticationStateProvider authStateProvider)
        {
            _http = http;
            _tokenStore = tokenStore;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            var outgoing = JsonSerializer.Serialize(request);
            Console.WriteLine("Register request payload: " + outgoing);

            var res = await _http.PostAsJsonAsync("Auth/v1/register", request);
            if (res.IsSuccessStatusCode) return res.IsSuccessStatusCode;
            else
            {
                string? content = await res.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    throw new HttpRequestException(content);
                }
                else
                {
                    throw new HttpRequestException("Unable to parse failure response from API.");
                }
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var outgoing = JsonSerializer.Serialize(request);
            Console.WriteLine("Login request payload: " + outgoing);

            var res = await _http.PostAsJsonAsync("Auth/v1/login", request);

            if (res == null)
            {
                throw new InvalidDataException("No response from API.");
            }
            else
            {
                try
                {
                    var dto = await res.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (dto?.AccessToken.Token is not null && dto.AccessToken.Token.Length > 0)
                    {
                        await _tokenStore.SetTokenAsync(dto.AccessToken.Token);
                        if (_authStateProvider is JwtAuthStateProvider jwtProv)
                            jwtProv.NotifyAuthStateChanged();

                        return dto;
                    }
                    else
                    {
                        throw new HttpRequestException("Token length null or invalid.");
                    }
                }
                catch
                {
                    string? content = await res.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        throw new HttpRequestException(content);
                    }
                    else
                    {
                        throw new HttpRequestException("Unable to parse failure response from API.");
                    }
                }
            }
        }

        public async Task LogoutAsync()
        {
            try { await _http.PostAsync("Auth/v1/logout", content: null); } catch { }

            await _tokenStore.ClearTokenAsync();
            if (_authStateProvider is JwtAuthStateProvider jwtProv)
                jwtProv.NotifyAuthStateChanged();
        }

        public async Task<bool> AccountCheckAsync()
        {
            try
            {
                var result = await _http.GetAsync("Auth/v1/accountcheck");
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return true;
            }
        }
    }
}
