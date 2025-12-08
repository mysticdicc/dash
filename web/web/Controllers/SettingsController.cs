using danklibrary.DankAPI;
using danklibrary.Settings;
using dankweb.API;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using web.Services;

namespace web.Controllers
{
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
            string content = System.IO.File.ReadAllText(_settingsLocation);
            return content;
        }

        [HttpPost]
        [Route("[controller]/v2/post/new")]
        public async Task<Results<BadRequest<string>, Ok<AllSettings>>> CreateNewSettings(AllSettings newSettings)
        {
            try
            {
                AllSettings.UpdateExistingSettingsFile(_settingsLocation, newSettings);
                _monitorService.Restart();
                return TypedResults.Ok(newSettings);
            }
            catch(Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
