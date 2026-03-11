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
        public DashboardSettings()
        {
            BackgroundImage = string.Empty;
            BackgroundImageDisplay64 = string.Empty;
        }
    }
}
