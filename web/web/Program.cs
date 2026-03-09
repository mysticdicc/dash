using dankweb.API;
using DashLib.Interfaces.Dashboard;
using DashLib.Interfaces.Monitoring;
using DashLib.Interfaces.Network;
using DashLib.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using web.Client.Services;
using web.Components;
using web.Data.Repos;
using web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);

SharedServices.Register(builder.Services, baseAddress);

builder.Services.AddDbContextFactory<DashDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

builder.Services.AddSingleton<ISubnetRepository, SubnetRepository>();
builder.Services.AddSingleton<IMonitoringRepository, MonitoringRepository>();
builder.Services.AddSingleton<IDashboardRepository, DashboardRepository>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MonitorService>();
builder.Services.AddHostedService(x => x.GetRequiredService<MonitorService>());

builder.Services.AddSingleton<AlertService>();
builder.Services.AddHostedService(x => x.GetRequiredService<AlertService>());

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

app.MapSwagger();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(web.Client._Imports).Assembly);

app.MapControllers();

app.Run();
