using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using web.Client.Auth;
using web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddScoped(sp =>
{
    var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);
    var handler = sp.GetRequiredService<AuthMessageHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler) { BaseAddress = baseAddress };
});

SharedServices.Register(builder.Services, baseAddress);

var host = builder.Build();

try
{
    var authProvider = host.Services.GetRequiredService<AuthenticationStateProvider>() as JwtAuthStateProvider;
    authProvider?.NotifyAuthStateChanged();
}
catch
{
    // ignore: resolution may fail during some hosting scenarios
}
