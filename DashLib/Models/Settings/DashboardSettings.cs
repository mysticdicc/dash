using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Settings
{
    public class DashboardSettings
    {
        public string BackgroundImage { get; set; }
        public string BackgroundImageDisplay64 { get; set; }
        public static string BackgroundImagePath = Path.Combine(AppContext.BaseDirectory, "wwwroot/background.jpg");
        public string HttpsCertBase64 { get; set; }
        public static string HttpsCertBasePath = Path.Combine(AppContext.BaseDirectory, "data/certs/tls.pfx");
        public string HttpsCertPassword { get; set; }
        public static string HttpsCertPasswordPath = Path.Combine(AppContext.BaseDirectory, "data/certs/tls.pfx.pass");

        public DashboardSettings()
        {
            BackgroundImage = string.Empty;
            BackgroundImageDisplay64 = string.Empty;
            HttpsCertBase64 = string.Empty;
            HttpsCertPassword = string.Empty;
        }
    }
}
