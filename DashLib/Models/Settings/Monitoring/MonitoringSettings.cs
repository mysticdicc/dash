using DashLib.Models.Settings.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Settings
{
    public class MonitoringSettings
    {
        public int PollingIntervalInSeconds { get; set; }
        public int AlertIntervalInSeconds { get; set; }
        public float AlertIfDownForPercent { get; set; }
        public bool AlertsEnabled { get; set; }
        public int AlertTimePeriodInMinutes { get; set; }
        public int AlertAgainAfterInMinutes { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public DiscordSettings DiscordSettings { get; set; }
        public TelegramSettings TelegramSettings { get; set; }
        
        public MonitoringSettings()
        {
        }

        public MonitoringSettings(bool isDefault)
        {
            PollingIntervalInSeconds = 600;
            AlertIntervalInSeconds = 600;
            AlertIfDownForPercent = 50.0F;
            AlertsEnabled = false;
            AlertTimePeriodInMinutes = 30;
            AlertAgainAfterInMinutes = 30;
            SmtpSettings = new SmtpSettings();
            DiscordSettings = new DiscordSettings();
            TelegramSettings = new TelegramSettings();
        }
    }
}
