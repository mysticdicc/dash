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
        public int MonitorStateRetentionPeriodInHours { get; set; }
        public SmtpSettings SmtpSettings { get; set; }
        public DiscordSettings DiscordSettings { get; set; }
        public TelegramSettings TelegramSettings { get; set; }
        public AlertSettings AlertSettings { get; set; }
        
        public MonitoringSettings()
        {
        }

        public MonitoringSettings(bool isDefault)
        {
            PollingIntervalInSeconds = 600;
            MonitorStateRetentionPeriodInHours = 144;
            SmtpSettings = new SmtpSettings();
            DiscordSettings = new DiscordSettings();
            TelegramSettings = new TelegramSettings();
            AlertSettings = new AlertSettings(true);
        }
    }
}
