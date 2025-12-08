using danklibrary.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using web.Services;

namespace web.Controllers
{
    [ApiController]
    public class SettingsController : Controller
    {
        private static string _settingsLocation = string.Empty;
        private readonly MonitorService _monitorService;

        public SettingsController(MonitorService monitorService)
        {
            _monitorService = monitorService;
            _settingsLocation = Path.Combine(AppContext.BaseDirectory, "settings.json");

            if (!System.IO.File.Exists(_settingsLocation))
            {
                try
                {
                    AllSettings.CreateNewSettingsFile(_settingsLocation);
                }
                catch { }
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/current")]
        public string GetCurrentSettings()
        {
            var content = AllSettings.GetCurrentSettingsFile(_settingsLocation);
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
                    settings.SubnetSettings == null)
                {
                    return TypedResults.BadRequest("One or more child objects is missing from settings.");
                }

                AllSettings.UpdateExistingSettingsFile(_settingsLocation, settings);
                _monitorService.Restart();
                return TypedResults.Ok(settings);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
