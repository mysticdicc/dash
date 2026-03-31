using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Settings.Monitoring
{
    public class AlertSettings
    {
        public bool Enabled { get; set; }
        public int AlertIntervalInSeconds { get; set; }
        public float AlertIfDownForPercent { get; set; }
        public bool IcmpDownPercentAlertsEnabled { get; set; }
        public bool IcmpDownOnceAlertsEnabled { get; set; }
        public bool TcpDownPercentAlertsEnabled { get; set; }
        public bool TcpDownOnceAlertsEnabled { get; set; }
        public int AlertTimePeriodInMinutes { get; set; }
        public int AlertAgainAfterInMinutes { get; set; }

        public AlertSettings() { }

        public AlertSettings(bool isDefault)
        {
            AlertIntervalInSeconds = 600;
            AlertIfDownForPercent = 50.0F;
            IcmpDownPercentAlertsEnabled = false;
            IcmpDownOnceAlertsEnabled = false;
            TcpDownPercentAlertsEnabled = false;
            TcpDownOnceAlertsEnabled = false;
            AlertTimePeriodInMinutes = 30;
            AlertAgainAfterInMinutes = 30;
        }
    }
}
