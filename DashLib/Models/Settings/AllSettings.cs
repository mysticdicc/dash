using DashLib.Models.Settings.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Settings
{
    public class AllSettings
    {
        public Guid Id { get; set; }
        public DashboardSettings DashboardSettings { get; set; }
        public MonitoringSettings MonitoringSettings { get; set; }
        public SubnetSettings SubnetSettings { get; set; }
        public LoggingSettings LoggingSettings { get; set; }

        [JsonIgnore] public static readonly string SettingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");

        [JsonIgnore] public static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public AllSettings() { }

        public AllSettings(bool isDefault)
        {
            Id = Guid.NewGuid();
            DashboardSettings = new DashboardSettings();
            MonitoringSettings = new MonitoringSettings(true);
            SubnetSettings = new SubnetSettings();
            LoggingSettings = new LoggingSettings();
        }

        static public AllSettings GetOrCreateDefaultSettingsFile()
        {
            AllSettings returnSettings = new(true);

            try
            {
                returnSettings = GetCurrentSettingsFile(SettingsPath);
            }
            catch
            {
                try
                {
                    CreateNewSettingsFile(SettingsPath);
                    returnSettings = GetCurrentSettingsFile(SettingsPath);
                }
                catch 
                { 
                    returnSettings = new AllSettings(true);
                }
            }

            return returnSettings;
        }

        static public void CreateNewSettingsFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var settings = new AllSettings(true);
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);
        }

        static public void UpdateExistingSettingsFile(string path, AllSettings newSettings)
        {
            newSettings.Normalize();
            var json = JsonSerializer.Serialize(newSettings, JsonOptions);
            File.WriteAllText(path, json);

            if (newSettings.DashboardSettings.BackgroundImage != null
                || newSettings.DashboardSettings.BackgroundImage != string.Empty)
            {
                if (File.Exists(DashboardSettings.BackgroundImagePath))
                {
                    File.Delete(DashboardSettings.BackgroundImagePath);
                }

                byte[] imageBytes = Convert.FromBase64String(newSettings.DashboardSettings.BackgroundImage!);
                File.WriteAllBytes(DashboardSettings.BackgroundImagePath, imageBytes);
            }
            else
            {
                if (File.Exists(DashboardSettings.BackgroundImagePath))
                {
                    File.Delete(DashboardSettings.BackgroundImagePath);
                }
            }
        }

        static public AllSettings GetCurrentSettingsFile(string path)
        {
            string content = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<AllSettings>(content, JsonOptions);
            if (settings != null)
            {
                settings.Normalize();
                return settings;
            }
            else
            {
                throw new FileLoadException("Json deserialization on settings file failed.");
            }
        }

        public void Normalize()
        {
            DashboardSettings ??= new DashboardSettings();
            MonitoringSettings ??= new MonitoringSettings(true);
            SubnetSettings ??= new SubnetSettings();
            LoggingSettings ??= new LoggingSettings();

            MonitoringSettings.SmtpSettings ??= new SmtpSettings();
            MonitoringSettings.DiscordSettings ??= new DiscordSettings();
            MonitoringSettings.TelegramSettings ??= new TelegramSettings();
        }
    }
}
