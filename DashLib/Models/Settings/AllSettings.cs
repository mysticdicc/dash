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
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                var settings = new AllSettings(true);
                var json = JsonSerializer.Serialize(settings, JsonOptions);
                File.WriteAllText(path, json);
            }
            catch
            {
                throw;
            }
        }

        static public void UpdateExistingSettingsFile(string path, AllSettings newSettings)
        {
            try
            {
                if (null != newSettings.MonitoringSettings && null != newSettings.DashboardSettings && null != newSettings.SubnetSettings)
                {
                    var json = JsonSerializer.Serialize(newSettings, JsonOptions);
                    File.WriteAllText(path, json);
                }
                else
                {
                    throw new InvalidCastException("One or more child objects is missing from settings");
                }
            }
            catch
            {
                throw;
            }
        }

        static public AllSettings GetCurrentSettingsFile(string path)
        {
            try
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
            catch
            {
                throw;
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

            try
            {
                UpdateExistingSettingsFile(SettingsPath, this);
            }
            catch { }
        }
    }
}
