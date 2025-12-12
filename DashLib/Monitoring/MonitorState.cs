using DashLib.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Monitoring
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
                    state.IP = IP.Clone(parent);
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
                    state.IP = IP.Clone(parent);
                }
            }

            return allStates;
        }

        static public List<MonitorState> CreateRandomListOfMonitorStates(IP ip)
        {
            int count = new Random().Next(0, 20);
            var list = new List<MonitorState>();

            for (int i = 0; i < count; i++)
            {
                var random = new Random().Next(0, 100);

                var monitorState = new MonitorState
                {
                    ID = i,
                    IP_ID = ip.ID,
                    SubmitTime = DateTime.Now.AddMinutes(-i * 5),
                    PortState = [],
                    IP = ip
                };

                if (random >= 0 && random < 50)
                {
                    monitorState.PingState = new PingState
                    {
                        ID = i,
                        MonitorID = i,
                        MonitorState = monitorState,
                        Response = true
                    };

                    if (random >= 0 && random < 25)
                    {
                        monitorState.PortState.Add(new PortState
                        {
                            ID = i,
                            MonitorID = i,
                            Port = 80,
                            Status = true,
                            MonitorState = monitorState
                        });
                        monitorState.PortState.Add(new PortState
                        {
                            ID = i + 1,
                            MonitorID = i,
                            Port = 443,
                            Status = true,
                            MonitorState = monitorState
                        });
                    }
                    else if (random >= 25 && random < 50)
                    {
                        monitorState.PortState.Add(new PortState
                        {
                            ID = i,
                            MonitorID = i,
                            Port = 80,
                            Status = false,
                            MonitorState = monitorState
                        });
                        monitorState.PortState.Add(new PortState
                        {
                            ID = i + 1,
                            MonitorID = i,
                            Port = 443,
                            Status = false,
                            MonitorState = monitorState
                        });
                    }
                }
                else if (random > 50 && random <= 100)
                {
                    monitorState.PingState = new PingState
                    {
                        ID = i,
                        MonitorID = i,
                        Response = false
                    };
                }

                if (null != monitorState.PingState || (monitorState.PortState != null && monitorState.PortState.Count > 0))
                {
                    list.Add(monitorState);
                }
            }

            if (list.Count > 0 && ip.IsMonitoredICMP == false)
            {
                ip.IsMonitoredICMP = true;

                if (list.Where(x => x.PortState != null).Select(x => x.PortState).Count() > 0 && ip.IsMonitoredTCP == false)
                {
                    ip.IsMonitoredTCP = true;
                }
            }
            else if (list.Count == 0)
            {
                list.Add(
                    new MonitorState { 
                        SubmitTime = DateTime.Now, 
                        IP_ID = ip.ID, 
                        IP = ip, 
                        ID = 1,
                        PingState = new PingState { 
                            ID = 1, 
                            MonitorID = 1, 
                            Response = true} 
                    });
            }

            ip.MonitorStateList = list;
            return list;
        }

        static public List<IP> GetMonitorStatesFromTimespan(List<IP> ips, TimeSpan timespan)
        {
            var ipList = new List<IP>();
            var oldDate = DateTime.Now - timespan;

            var states = GetAllDevicePollsFromIps(ips);
            var validStates = states.Where(x => x.SubmitTime.ToUniversalTime() >= oldDate.ToUniversalTime()).ToList();

            foreach (var state in validStates)
            {
                var parent = ips.Where(x => x.ID == state.IP_ID).FirstOrDefault();

                if (null != parent)
                {
                    var exists = ipList.Where(x => x.ID == parent.ID).ToList();
                    IP? clone;

                    if (exists.Count == 0)
                    {
                        clone = IP.Clone(parent);
                        ipList.Add(clone);

                        state.IP = clone;

                        if (null == clone.MonitorStateList)
                        {
                            clone.MonitorStateList = [];
                        }

                        if (!clone.MonitorStateList.Contains(state))
                        {
                            clone.MonitorStateList.Add(state);
                        }
                    }
                    else if (exists.Count > 0)
                    {
                        clone = exists[0];

                        state.IP = clone;

                        if (null == clone.MonitorStateList)
                        {
                            clone.MonitorStateList = [];
                        }

                        if (!clone.MonitorStateList.Contains(state))
                        {
                            clone.MonitorStateList.Add(state);
                        }
                    }
                }
            }

            return ipList;
        }
    }
}
