using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models.Settings.Monitoring
{
    public class TelegramSettings
    {
        public bool AlertsEnabled { get; set; }
        public string BotToken { get; set; }
        public string ChatId { get; set; }

        [JsonConstructor] public TelegramSettings(bool AlertsEnabled, string BotToken, string ChatId)
        {
            this.AlertsEnabled = AlertsEnabled;
            this.BotToken = BotToken;
            this.ChatId = ChatId;

        }

        public TelegramSettings()
        {
            AlertsEnabled = false;
            BotToken = string.Empty;
            ChatId = string.Empty;
        }
    }
}
