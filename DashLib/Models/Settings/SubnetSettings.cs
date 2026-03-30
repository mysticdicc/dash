using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Settings
{
    public class SubnetSettings
    {
        public byte[] PrimaryDnsServerAsByte { get; set; }
        [JsonIgnore] public string PrimaryDnsServerAsString
        {
            get
            {
                return IpMonitoringTarget.ConvertToString(PrimaryDnsServerAsByte);
            }
            set
            {
                PrimaryDnsServerAsByte = IpMonitoringTarget.ConvertToByte(value);
            }
        }
        public int PrimaryDnsPort { get; set; }
        public byte[] SecondaryDnsServerAsByte { get; set; }
        [JsonIgnore] public string SecondaryDnsServerAsString
        {
            get
            {
                return IpMonitoringTarget.ConvertToString(SecondaryDnsServerAsByte);
            }
            set
            {
                SecondaryDnsServerAsByte = IpMonitoringTarget.ConvertToByte(value);
            }
        }
        public int SecondaryDnsPort { get; set; }

        public SubnetSettings()
        {
            PrimaryDnsServerAsByte = IpMonitoringTarget.ConvertToByte("1.1.1.1");
            PrimaryDnsPort = 53;
            SecondaryDnsServerAsByte = IpMonitoringTarget.ConvertToByte("8.8.8.8");
            SecondaryDnsPort = 53;
        }

        public IPAddress GetPrimaryDnsServer()
        {
            return IPAddress.Parse(this.PrimaryDnsServerAsString);
        }

        public IPAddress GetSecondaryDnsServer()
        {
            return IPAddress.Parse(this.SecondaryDnsServerAsString);
        }
    }
}
