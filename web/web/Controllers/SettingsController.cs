using DashLib.Models.Settings;
using DashLib.Models.Settings.Monitoring;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web.Services;

namespace web.Controllers
{
    [ApiController]
    public class SettingsController : Controller
    {
        private readonly MonitorService _monitorService;
        private readonly SettingsService _settingsService;
        private readonly TelegramService _telegramService;
        private readonly DiscordService _discordService;
        private readonly DiscoveryService _discoveryService;
        public SettingsController(
            MonitorService monitorService, 
            SettingsService settingsService,
            TelegramService telegramService,
            DiscordService discordService,
            DiscoveryService discoveryService
            )
        {
            _monitorService = monitorService;
            _settingsService = settingsService;
            _telegramService = telegramService;
            _discordService = discordService;
            _discoveryService = discoveryService;
        }

        [HttpGet]
        [Route("[controller]/v2/get/current")]
        public string GetCurrentSettings()
        {
            var content = AllSettings.GetOrCreateDefaultSettingsFile();

            if (!string.IsNullOrEmpty(content.MonitoringSettings.SmtpSettings.Password))
            {
                var unencrypted = SmtpSettings.DecryptPassword(content.MonitoringSettings.SmtpSettings);
                content.MonitoringSettings.SmtpSettings = unencrypted;
            }

            return JsonSerializer.Serialize(content, AllSettings.JsonOptions);
        }

        [HttpPost]
        [Route("[controller]/v2/post/new")]
        public async Task<Results<BadRequest<string>, Ok<AllSettings>>> CreateNewSettings(AllSettings settings)
        {
            try
            {
                if (settings == null ||
                    settings.DashboardSettings == null ||
                    settings.MonitoringSettings == null ||
                    settings.SubnetSettings == null ||
                    settings.MonitoringSettings.SmtpSettings == null ||
                    settings.MonitoringSettings.DiscordSettings == null ||
                    settings.MonitoringSettings.TelegramSettings == null)
                {
                    return TypedResults.BadRequest("One or more child objects is missing from settings.");
                }

                if (settings.MonitoringSettings.SmtpSettings.Password != string.Empty)
                {
                    var encrypted = SmtpSettings.EncryptPassword(settings.MonitoringSettings.SmtpSettings);
                    settings.MonitoringSettings.SmtpSettings = encrypted;
                }

                AllSettings.UpdateExistingSettingsFile(AllSettings.SettingsPath, settings);
                _monitorService.Restart();
                await _settingsService.RefreshSettingsAsync(CancellationToken.None);
                await _telegramService.Restart();
                await _discordService.Restart();
                await _discoveryService.Restart();
                return TypedResults.Ok(settings);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
