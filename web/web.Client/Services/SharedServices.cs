using danklibrary.DankAPI;
using ApexCharts;

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

            services.AddSingleton<DashAPI>();
            services.AddSingleton<IDashAPI, DashboardApiService>();

            services.AddSingleton<MonitoringAPI>();
            services.AddSingleton<IMonitoringAPI, MonitoringApiService>();

            services.AddSingleton<SubnetsAPI>();
            services.AddScoped<ISubnetsAPI, SubnetApiService>();

            services.AddSingleton<SettingsAPI>();
            services.AddScoped<ISettingsAPI, SettingsApiService>();
        }
    }
}
