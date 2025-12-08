using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);

builder.Services.AddSingleton(sp =>
{
    return new HttpClient { BaseAddress = baseAddress };
});

SharedServices.Register(builder.Services, baseAddress);

await builder.Build().RunAsync();
