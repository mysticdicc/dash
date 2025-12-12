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
            SmtpServerAddress = string.Empty;
            SmtpUsername = string.Empty;
            SmtpPassword = string.Empty;
            SmtpPort = 25;
            SmtpTargetEmail = string.Empty;
            SmtpAuthenticationIsRequired = false;
        }
    }
}
