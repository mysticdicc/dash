using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using DashLib.Monitoring;
using DashLib.Dashboard;

namespace DashLib.Network
{
    public class IP
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        required public byte[] Address { get; set; }
        public string? Hostname { get; set; }
        public int SubnetID { get; set; }
        public List<MonitorState>? MonitorStateList { get; set; }
        public bool IsMonitoredICMP { get; set; }
        public bool IsMonitoredTCP { get; set; }
        public List<int>? PortsMonitored { get; set; }
        public DateTime LastAlertSent { get; set;  }

        //ef mapping
        public virtual DeviceStatusWidget? DeviceStatusWidget { get; }
        [JsonIgnore] public virtual Subnet? Subnet { get; }
        [JsonIgnore] public virtual MonitorState? MonitorState { get; set; }

        static public byte[] ConvertToByte(IPAddress ip)
        {
            return IPAddress.Parse(ip.ToString()).GetAddressBytes();
        }

        static public byte[] ConvertToByte(string ip)
        {
            return IPAddress.Parse(ip).GetAddressBytes();
        }

        static public byte[] GetMaskFromCidr(int cidr)
        {
            var mask = (cidr == 0) ? 0 : uint.MaxValue << (32 - cidr);
            return BitConverter.GetBytes(mask).Reverse().ToArray();
        }

        static public string ConvertToString(byte[] ip)
        {
            var temp = new IPAddress(ip);
            return temp.ToString();
        }

        static public IP Clone(IP ip)
        {
            var _ip = new IP
            {
                Address = ip.Address,
                ID = ip.ID,
                IsMonitoredICMP = ip.IsMonitoredICMP,
                IsMonitoredTCP = ip.IsMonitoredTCP,
                PortsMonitored = ip.PortsMonitored,
                Hostname = ip.Hostname,
                SubnetID = ip.SubnetID,
                LastAlertSent = ip.LastAlertSent
            };

            return _ip;
        }
    }
}
