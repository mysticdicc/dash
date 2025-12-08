using danklibrary.Monitoring;
using dankweb.API;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using web.Client.Services;
using web.Components;
using web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);

SharedServices.Register(builder.Services, baseAddress);

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<danknetContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

builder.Services.AddSingleton<MonitorService>();
builder.Services.AddHostedService(x => x.GetRequiredService<MonitorService>());

builder.Services.AddSingleton<DiscoveryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(web.Client._Imports).Assembly);

app.MapControllers();

app.Run();
