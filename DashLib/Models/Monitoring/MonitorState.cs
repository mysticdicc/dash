using DashLib.Models.Network;
using DashLib.Models.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Monitoring
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

            List<IP> _parentList = [];

            foreach (var state in allStates)
            {
                var parent = ips.FirstOrDefault(x => x.ID == state.IP_ID);

                if (parent != null)
                {
                    var inList = _parentList.Where(x => x.ID == state.IP_ID).FirstOrDefault();

                    if (inList == null)
                    {
                        _parentList.Add(IP.Clone(parent));
                    }
                    else
                    {
                        state.IP = inList;
                    }
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

            List<IP> _parentList = [];

            foreach (var state in allStates)
            {
                var parent = ips.FirstOrDefault(x => x.ID == state.IP_ID);

                if (parent != null)
                {
                    var inList = _parentList.Where(x => x.ID == state.IP_ID).FirstOrDefault();

                    if (inList == null)
                    {
                        _parentList.Add(IP.Clone(parent));
                    }
                    else
                    {
                        state.IP = inList;
                    }
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

        static public string GetMonitorStateSummaryFromIps(List<IP> ips, AllSettings currentSettings)
        {
            int totalOnline = 0;
            int totalOffline = 0;
            float totalUptime = 0;

            var sb = new StringBuilder();
            sb.AppendLine("Current Monitoring Settings:");
            sb.AppendLine($"Evaluation Period: {currentSettings.MonitoringSettings.AlertTimePeriodInMinutes} minutes");
            sb.AppendLine($"Alert Threshold: {currentSettings.MonitoringSettings.AlertIfDownForPercent}%");
            sb.AppendLine($"Alerts Enabled: {currentSettings.MonitoringSettings.AlertsEnabled}");
            sb.AppendLine($"SMTP Alerts: {currentSettings.MonitoringSettings.SmtpSettings.AlertsEnabled}");
            sb.AppendLine($"Discord Alerts: {currentSettings.MonitoringSettings.DiscordSettings.AlertsEnabled}");
            sb.AppendLine();

            var timespan = TimeSpan.FromMinutes(currentSettings.MonitoringSettings.AlertTimePeriodInMinutes);
            var oldDate = DateTime.UtcNow - timespan;

            foreach (var ip in ips)
            {
                sb.AppendLine($"IP: {IP.ConvertToString(ip.Address)}");

                try
                {
                    var list = new List<IP>() { ip };
                    var states = GetAllDevicePollsFromIps(list).Where(x => null != x.PingState).OrderByDescending(x => x.SubmitTime);
                    var lastState = states.FirstOrDefault();

                    if (null == lastState)
                    {
                        sb.AppendLine("Status: Offline | No Monitor State");
                        totalOffline++;
                    }
                    else
                    {
                        if (null == lastState.PingState)
                        {
                            sb.AppendLine("Status: Offline | No Ping State");
                            totalOffline++;
                        }
                        else
                        {
                            if (lastState.PingState.Response)
                            {
                                sb.AppendLine("Status: Online");
                                totalOnline++;
                            }
                            else
                            {
                                sb.AppendLine("Status: Offline");
                                totalOffline++;
                            }
                        }
                    }

                    var pingStates = states
                        .Where(x => x.PingState != null)
                        .Where(x => x.SubmitTime > oldDate)
                        .OrderBy(x => x.SubmitTime)
                        .ToList();

                    int totalCount = pingStates.Count();
                    int upCount = pingStates.Where(x => x.PingState!.Response == true).ToList().Count();

                    if (totalCount <= 0)
                    {
                        totalCount = 1;
                    }

                    float uptimePercent = (upCount / totalCount) * 100;
                    totalUptime += uptimePercent;

                    sb.AppendLine($"Uptime Percentage: {(float)uptimePercent}%");
                }
                catch (Exception ex)
                {
                    sb.AppendLine("Status: Offline | No Monitor State");
                    sb.AppendLine($"Error fetching uptime percentage: {ex.Message}");
                }

                sb.AppendLine();
            }

            sb.AppendLine($"Total Online: {totalOnline}");
            sb.AppendLine($"Total Offline: {totalOffline}");
            sb.AppendLine($"Average Uptime: {(totalUptime / (ips.Count))}%");

            return sb.ToString();
        }

        static public string GetDowntimeAlertFromIps(List<IP> ips)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Device Downtime Alert");
            sb.AppendLine($"Offline Devices: {ips.Count}");
            sb.AppendLine();

            var lastMonitorStates = GetLastDevicePollsFromIps(ips);

            foreach (var state in lastMonitorStates)
            {
                sb.AppendLine($"IP: {IP.ConvertToString(state.IP.Address)}");
                sb.AppendLine($"Last Poll Time: {state.SubmitTime}");
                sb.AppendLine($"Last Status: {(state.PingState!.Response == true ? "Online" : "Offline")}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
