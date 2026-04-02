using Microsoft.JSInterop;

namespace web.Client.Services
{
    public class TokenStorageService(IJSRuntime js)
    {
        private const string TokenKey = "auth_token";
        private readonly IJSRuntime _js = js;

        public async Task<string?> GetTokenAsync()
        {
            if (!OperatingSystem.IsBrowser())
                return null;

            return await _js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }

        public async Task SetTokenAsync(string token)
        {
            if (!OperatingSystem.IsBrowser())
                return;

            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        }

        public async Task ClearTokenAsync()
        {
            if (!OperatingSystem.IsBrowser())
                return;

            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        }
    }
}