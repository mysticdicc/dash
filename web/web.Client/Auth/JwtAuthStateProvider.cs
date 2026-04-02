using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using web.Client.Services;

namespace web.Client.Auth
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthTokenService _storageService;

        public JwtAuthStateProvider(AuthTokenService storageService)
        {
            _storageService = storageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _storageService.GetTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyAuthStateChanged()
            => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2)
                yield break;

            var payload = parts[1]
                .PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
                .Replace('-', '+')
                .Replace('_', '/');

            string json;
            try
            {
                json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            }
            catch
            {
                yield break;
            }

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();

            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in kvp.Value.EnumerateArray())
                        yield return new Claim(kvp.Key, item.ToString());
                }
                else
                {
                    yield return new Claim(kvp.Key, kvp.Value.ToString());
                }
            }
        }
    }
}