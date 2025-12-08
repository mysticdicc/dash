using danklibrary.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace danklibrary.Monitoring
{
    public class MonitorState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int IP_ID { get; set; }
        [JsonIgnore]
        public IP? IP { get; set; }

        public required DateTime SubmitTime { get; set; }
        public PingState? PingState { get; set; }
        public List<PortState>? PortState { get; set; }

        static public List<MonitorState> GetLastDevicePollsFromIps(List<IP> ips)
        {
            var monitoredIps = ips
                .Where(ip => (ip.IsMonitoredTCP || ip.IsMonitoredICMP) && ip.MonitorStateList != null)
                .ToList();

            var allStates = monitoredIps
                .SelectMany(ip => ip.MonitorStateList!)
                .Where(x => x.PingState != null || x.PortState != null)
                .ToList();

            allStates = allStates
                .GroupBy(x => x.IP_ID)
                .Select(g => g.OrderByDescending(ms => ms.SubmitTime).First())
                .ToList();

            foreach (var state in allStates)
            {
                var parent = ips.FirstOrDefault(x => x.ID == state.IP_ID);

                if (parent != null)
                {
                    state.IP = parent;
                }
            }

            return allStates;
        }

        static public List<MonitorState> GetAllDevicePollsFromIps(List<IP> ips)
        {
            var monitoredIps = ips
                .Where(ip => (ip.IsMonitoredTCP || ip.IsMonitoredICMP) && ip.MonitorStateList != null)
                .ToList();

            var allStates = monitoredIps
                .SelectMany(ip => ip.MonitorStateList!)
                .Where(x => x.PingState != null || x.PortState != null)
                .ToList();

            foreach (var state in allStates)
            {
                var parent = ips.FirstOrDefault(x => x.ID == state.IP_ID);

                if (parent != null)
                {
                    state.IP = parent;
                }
            }

            return allStates;
        }
    }
}
