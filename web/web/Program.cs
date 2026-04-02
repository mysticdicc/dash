using dankweb.API;
using DashLib.Interfaces.Dashboard;
using DashLib.Interfaces.Logging;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Auth;
using DashLib.Models.Monitoring;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using web.Client.Services;
using web.Components;
using web.Data.Repos;
using web.Hubs;
using web.Middleware;
using web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var baseAddress = new Uri(builder.Configuration.GetValue<string>("WorkerApiAddress")!);

builder.Services.AddSignalR();

builder.Services.AddDbContext<DashDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")),
    contextLifetime: ServiceLifetime.Scoped,
    optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddDbContextFactory<DashDbContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<DashDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

builder.Services.AddAuthorization();

var jwtKey = builder.Configuration["Auth:JwtKey"]!;
var jwtIssuer = builder.Configuration["Auth:Issuer"]!;
var jwtAudience = builder.Configuration["Auth:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/loghub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<LoggingService>();

SharedServices.Register(builder.Services, baseAddress);
builder.Services.AddSingleton<ILoggingRepository, LoggingRepository>();
builder.Services.AddSingleton<IMonitorTargetRepository, MonitorTargetRepository>();
builder.Services.AddSingleton<IMonitorStateRepository, MonitorStateRepository>();
builder.Services.AddSingleton<IDashboardRepository, DashboardRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SettingsService>();
builder.Services.AddSingleton<DiscordService>();
builder.Services.AddSingleton<TelegramService>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddSingleton<DiscoveryService>();

builder.Services.AddHostedService<MonitorService>();
builder.Services.AddHostedService<AlertService>();
builder.Services.AddHostedService<CleanupService>();

builder.Services.AddControllers();

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);

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
app.UseMiddleware<ApiLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

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

app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(web.Client._Imports).Assembly)
    .AllowAnonymous();

app.MapControllers();
app.MapHub<LogHub>("/loghub");

app.Run();

