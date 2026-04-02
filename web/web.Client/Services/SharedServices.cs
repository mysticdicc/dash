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
            services.AddAuthorizationCore();

            services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
            services.AddScoped<AuthTokenService>();
            services.AddScoped<AuthMessageHandler>();
            services.AddScoped<AuthApiService>();
            services.AddScoped<RefreshTokenService>();

            services.AddHttpClient("AuthorizedClient", client =>
            {
                client.BaseAddress = baseAddress;
            })
            .AddHttpMessageHandler<AuthMessageHandler>();

            services.AddScoped(sp =>
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthorizedClient"));

            services.AddHttpClient("NoAuthClient", client =>
            {
                client.BaseAddress = baseAddress;
            });


            services.AddApexCharts();
            services.AddSingleton<NotificationService>();

            services.AddScoped<LoggingAPI>();
            services.AddScoped<ILoggingAPI, LoggingApiService>();

            services.AddScoped<DashAPI>();
            services.AddScoped<IDashAPI, DashboardApiService>();

            services.AddScoped<MonitorStateAPI>();
            services.AddScoped<IMonitorStatesAPI, MonitorStateApiService>();

            services.AddScoped<MonitorTargetAPI>();
            services.AddScoped<IMonitorTargetAPI, MonitorTargetApiService>();

            services.AddScoped<SettingsAPI>();
            services.AddScoped<ISettingsAPI, SettingsApiService>();

            services.AddScoped<MailAPI>();
            services.AddScoped<IMailAPI, MailApiService>();

            services.AddScoped<LoggingHubService>(sp =>
                new LoggingHubService(
                    sp.GetRequiredService<ILoggingAPI>(),
                    baseAddress.ToString(), sp.
                    GetRequiredService<AuthTokenService>()
                ));


        }
    }
}
