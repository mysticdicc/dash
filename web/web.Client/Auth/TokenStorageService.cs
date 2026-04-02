using Microsoft.JSInterop;

namespace web.Client.Services
{
    public class TokenStorageService(IJSRuntime js)
    {
        private const string TokenKey = "auth_token";
        private readonly IJSRuntime _js = js;

        public async Task<string?> GetTokenAsync()
        {
            var result = await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
            return result;
        }

        public async Task SetTokenAsync(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task ClearTokenAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }
}