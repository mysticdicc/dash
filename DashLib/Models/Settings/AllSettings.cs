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
            settings.Normalize();
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);
        }

        static private bool ShouldSaveFile(string current, string newString) 
        {
            if (!string.IsNullOrEmpty(current) && !string.IsNullOrEmpty(newString))
            {
                if (current != newString) return true;
                else return false;
            }
            else
            {
                if (string.IsNullOrEmpty(current) && string.IsNullOrEmpty(newString)) return false;
                if (string.IsNullOrEmpty(current) && !string.IsNullOrEmpty(newString)) return true;
                if (!string.IsNullOrEmpty(current) && string.IsNullOrEmpty(newString)) return false;
                return false;
            }
        }

        static public void UpdateExistingSettingsFile(string path, AllSettings newSettings)
        {
            var currentSettings = GetCurrentSettingsFile(path);
            newSettings.Normalize();

            if (ShouldSaveFile(currentSettings.DashboardSettings.BackgroundImage, newSettings.DashboardSettings.BackgroundImage))
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

            if (ShouldSaveFile(currentSettings.DashboardSettings.HttpsCertBase64, newSettings.DashboardSettings.HttpsCertBase64))
            {
                if (File.Exists(DashboardSettings.HttpsCertBasePath))
                {
                    File.Delete(DashboardSettings.HttpsCertBasePath);
                }

                byte[] pfxBytes = Convert.FromBase64String(newSettings.DashboardSettings.HttpsCertBase64);
                File.WriteAllBytes(DashboardSettings.HttpsCertBasePath, pfxBytes);
            }
            else
            {
                if (File.Exists(DashboardSettings.HttpsCertBasePath))
                {
                    File.Delete(DashboardSettings.HttpsCertBasePath);
                }
            }

            if (ShouldSaveFile(currentSettings.DashboardSettings.HttpsCertPassword, newSettings.DashboardSettings.HttpsCertPassword))
            {
                if (File.Exists(DashboardSettings.HttpsCertPasswordPath))
                {
                    File.Delete(DashboardSettings.HttpsCertPasswordPath);
                }

                try
                {
                    File.WriteAllText(DashboardSettings.HttpsCertPasswordPath, newSettings.DashboardSettings.HttpsCertPassword);
                    newSettings.DashboardSettings.HttpsCertPassword = string.Empty;
                }
                catch
                {
                    newSettings.DashboardSettings.HttpsCertPassword = "Failed to save.";
                }
            }
            else
            {
                if (File.Exists(DashboardSettings.HttpsCertPasswordPath))
                {
                    File.Delete(DashboardSettings.HttpsCertPasswordPath);
                }
            }

            var json = JsonSerializer.Serialize(newSettings, JsonOptions);
            File.WriteAllText(path, json);
        }

        static public AllSettings GetCurrentSettingsFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new InvalidDataException($"Path {path} does not exist.");
            }

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

            MonitoringSettings.AlertSettings ??= new AlertSettings(true);
            MonitoringSettings.SmtpSettings ??= new SmtpSettings();
            MonitoringSettings.DiscordSettings ??= new DiscordSettings();
            MonitoringSettings.TelegramSettings ??= new TelegramSettings();
        }
    }
}
