using DashLib.Interfaces.Network;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Models.Monitoring
{
    public class PortState : BaseMonitorState, IMonitorState<PortState>
    {
        public int TargetPort { get; set; }

        public PortState(BaseMonitoringTarget target) : base(target) { }

        public PortState(BaseMonitoringTarget target, DateTime timeStamp) : base(target, timeStamp) { }

        public PortState(BaseMonitoringTarget target, int port) : base(target)
        {
            TargetPort = port;
        }

        public PortState(BaseMonitoringTarget target, DateTime timeStamp, int port) : base(target, timeStamp)
        {
            TargetPort = port;
        }

        static public List<PortState> GetMonitorStatesFromListIp(List<IpMonitoringTarget> ipTargets)
        {
            var states = ipTargets.SelectMany(x => x.TcpMonitorStates).ToList();
            return states ?? [];
        }

        static public List<PortState> GetMonitorStatesFromListDns(List<DnsMonitoringTarget> dnsTargets)
        {
            var states = dnsTargets.SelectMany(x => x.TcpMonitorStates).ToList();
            return states ?? [];
        }

        static public List<PortState> GetMostRecentStatesFromListIp(List<IpMonitoringTarget> ipMonitoringTargets)
        {
            return ipMonitoringTargets
                .SelectMany(t => t.TcpMonitorStates)
                .GroupBy(s => (s.TargetId, s.TargetPort))
                .Select(g => g.MaxBy(x => x.Timestamp))
                .Where(s => s != null)
                .ToList()!;
        }

        static public List<PortState> GetMostRecentStatesFromListDns(List<DnsMonitoringTarget> dnsMonitoringTargets)
        {
            return dnsMonitoringTargets
                .SelectMany(t => t.TcpMonitorStates)
                .GroupBy(s => (s.TargetId, s.TargetPort))
                .Select(g => g.MaxBy(x => x.Timestamp))
                .Where(s => s != null)
                .ToList()!;
        }
    }
}
