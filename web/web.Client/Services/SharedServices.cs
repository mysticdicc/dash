using DashLib.DankAPI;
using ApexCharts;
using DashLib.Interfaces;
using DashLib.API;
using DashLib.Interfaces.Dashboard;
using DashLib.Interfaces.Monitoring;
using DashLib.Interfaces.Logging;

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

            services.AddSingleton<LoggingHubService>(sp =>
                new LoggingHubService(sp.GetRequiredService<ILoggingAPI>(), baseAddress.ToString()));

            services.AddSingleton<DashAPI>();
            services.AddSingleton<IDashAPI, DashboardApiService>();

            services.AddSingleton<MonitorStateAPI>();
            services.AddSingleton<IMonitorStatesAPI, MonitoringApiService>();

            services.AddSingleton<MonitorTargetAPI>();
            services.AddScoped<IMonitorTargetAPI, SubnetApiService>();

            services.AddSingleton<SettingsAPI>();
            services.AddScoped<ISettingsAPI, SettingsApiService>();

            services.AddSingleton<MailAPI>();
            services.AddScoped<IMailAPI, MailApiService>();
        }
    }
}
