using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models
{
    public class LogEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }
        public LogLevel Level { get; set; }
        public enum LogSource
        {
            AlertService,
            MonitoringService,
            DiscordService,
            TelegramService,
            SettingsService,
            DiscoveryService,
            ApiController
        }
        public LogSource Source { get; set; }

        [JsonConstructor] public LogEntry(int Id, DateTime Timestamp, string Message, LogLevel Level, LogSource Source)
        {
            this.Id = Id;
            this.Timestamp = Timestamp;
            this.Message = Message;
            this.Level = Level;
            this.Source = Source;
        }

        public LogEntry(LogLevel level, LogSource source, string message)
        {
            Timestamp = DateTime.Now;
            Message = message;
            Level = level;
            Source = source;
        }
    }
}
