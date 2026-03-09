using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Settings.Monitoring
{
    public class DiscordSettings
    {
        public bool AlertsEnabled { get; set; }
        public string Token { get; set; }
        public ulong ChannelID { get; set; }

        public DiscordSettings()
        {
            AlertsEnabled = false;
            Token = string.Empty;
            ChannelID = 0;
        }
    }
}
