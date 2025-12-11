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

        public MonitoringSettings()
        {
        }

        public MonitoringSettings(bool isDefault)
        {
            PollingIntervalInSeconds = 600;
        }
    }
}
