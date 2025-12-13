using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Settings
{
    public class MonitoringSettings
    {
        public int PollingIntervalInSeconds { get; set; }
        public int AlertIntervalInSeconds { get; set; }
        public float AlertIfDownForPercent { get; set; }
        public bool AlertsEnabled { get; set; }
        public int AlertTimePeriodInMinutes { get; set; }
        public int AlertAgainAfterInMinutes { get; set; }
        public string SmtpServerAddress { get; set; }
        public bool SmtpAuthenticationIsRequired { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpTargetEmail { get; set; }
        
        public MonitoringSettings()
        {
        }

        public MonitoringSettings(bool isDefault)
        {
            PollingIntervalInSeconds = 600;
            AlertIntervalInSeconds = 600;
            AlertIfDownForPercent = 50.0F;
            AlertsEnabled = false;
            SmtpServerAddress = string.Empty;
            SmtpUsername = string.Empty;
            SmtpPassword = string.Empty;
            SmtpPort = 25;
            SmtpTargetEmail = string.Empty;
            SmtpAuthenticationIsRequired = false;
            AlertTimePeriodInMinutes = 30;
            AlertAgainAfterInMinutes = 30;
        }

        static public MonitoringSettings EncryptPassword(MonitoringSettings settings)
        {
            if (null != settings.SmtpPassword && settings.SmtpPassword != string.Empty)
            {
                settings.SmtpPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.SmtpPassword));
            }
            return settings;
        }

        static public MonitoringSettings DecryptPassword(MonitoringSettings settings)
        {
            if (null != settings.SmtpPassword && settings.SmtpPassword != string.Empty)
            {
                settings.SmtpPassword = Encoding.UTF8.GetString(Convert.FromBase64String(settings.SmtpPassword));
            }
            return settings;
        }
    }
}
