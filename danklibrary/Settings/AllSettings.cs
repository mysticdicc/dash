using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace danklibrary.Settings
{
    public class AllSettings
    {
        public Guid Id { get; set; }
        public DashboardSettings DashboardSettings { get; set; }
        public MonitoringSettings MonitoringSettings { get; set; }
        public SubnetSettings SubnetSettings { get; set; }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        public AllSettings()
        {
            Id = new Guid();
            DashboardSettings = new();
            MonitoringSettings = new();
            SubnetSettings = new();
        }

        static public void CreateNewSettingsFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            var settings = new AllSettings();
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);

        }

        static public void UpdateExistingSettingsFile(string path, AllSettings newSettings)
        {
            var json = JsonSerializer.Serialize(newSettings, JsonOptions);
            File.WriteAllText(path, json);
        }

        static public AllSettings GetCurrentSettingsFile(string path)
        {
            string content = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<AllSettings>(content, JsonOptions);

            if (null != settings)
            {
                return settings;
            }
            else
            {
                throw new FileLoadException("Json deserialization on settings file failed.");
            }
        }
    }
}
