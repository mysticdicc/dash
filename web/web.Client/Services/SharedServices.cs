using ApexCharts;
using DashLib.API;
using DashLib.DankAPI;
using DashLib.Interfaces;
using DashLib.Interfaces.Dashboard;
using DashLib.Interfaces.Logging;
using DashLib.Interfaces.Monitoring;
using Microsoft.AspNetCore.Components.Authorization;
using web.Client.Auth;

namespace web.Client.Services
{
    public class SharedServices
    {
        public static void Register(IServiceCollection services, Uri baseAddress)
        {
            var client = new HttpClient() { BaseAddress = baseAddress };
            services.AddSingleton<HttpClient>(client);

            services.AddApexCharts();
            services.AddSingleton<NotificationService>();

            services.AddSingleton<LoggingAPI>();
            services.AddSingleton<ILoggingAPI, LoggingApiService>();

            services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
            services.AddScoped<TokenStorageService>();
            services.AddScoped<AuthApiService>();

            services.AddScoped<LoggingHubService>(sp =>
                new LoggingHubService(
                    sp.GetRequiredService<ILoggingAPI>(), 
                    baseAddress.ToString(), sp.
                    GetRequiredService<TokenStorageService>()
                ));

            services.AddSingleton<DashAPI>();
            services.AddSingleton<IDashAPI, DashboardApiService>();

            services.AddSingleton<MonitorStateAPI>();
            services.AddSingleton<IMonitorStatesAPI, MonitorStateApiService>();

            services.AddSingleton<MonitorTargetAPI>();
            services.AddScoped<IMonitorTargetAPI, MonitorTargetApiService>();

            services.AddSingleton<SettingsAPI>();
            services.AddScoped<ISettingsAPI, SettingsApiService>();

            services.AddSingleton<MailAPI>();
            services.AddScoped<IMailAPI, MailApiService>();
        }
    }
}
