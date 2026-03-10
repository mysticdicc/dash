using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Settings
{
    public class LoggingSettings
    {
        public bool LogsEnabled { get; set; }
        public int RetentionPeriodHours { get; set; }
        public bool LogInformationEnabled { get; set; }
        public bool LogWarningEnabled { get; set; }
        public bool LogErrorEnabled { get; set; }
        public bool LogApiEventsEnabled { get; set; }
        public bool LogAlertEventsEnabled { get; set; }
        public bool LogMonitoringEventsEnabled { get; set; }
        public bool LogDiscordEventsEnabled { get; set; }
        public bool LogTelegramEventsEnabled { get; set; }
        public bool LogSettingsEventsEnabled { get; set; }
        public bool LogDiscoveryEventsEnabled { get; set; }

        public LoggingSettings()
        {
            LogsEnabled = true;
            RetentionPeriodHours = 24;
            LogInformationEnabled = false;
            LogWarningEnabled = true;
            LogErrorEnabled = true;
            LogApiEventsEnabled = true;
            LogAlertEventsEnabled = true;
            LogMonitoringEventsEnabled = true;
            LogDiscordEventsEnabled = true;
            LogTelegramEventsEnabled = true;
            LogSettingsEventsEnabled = true;
            LogDiscoveryEventsEnabled = true;
        }
    }
}
