using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Settings.Monitoring
{
    public class SmtpSettings
    {
        public bool AlertsEnabled { get; set; }
        public string ServerAddress { get; set; }
        public bool AuthenticationIsRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string TargetEmail { get; set; }

        public SmtpSettings()
        {
            ServerAddress = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Port = 25;
            TargetEmail = string.Empty;
            AuthenticationIsRequired = false;
        }

        static public SmtpSettings EncryptPassword(SmtpSettings settings)
        {
            if (null != settings.Password && settings.Password != string.Empty)
            {
                settings.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(settings.Password));
            }
            return settings;
        }

        static public SmtpSettings DecryptPassword(SmtpSettings settings)
        {
            if (null != settings.Password && settings.Password != string.Empty)
            {
                settings.Password = Encoding.UTF8.GetString(Convert.FromBase64String(settings.Password));
            }
            return settings;
        }
    }
}
