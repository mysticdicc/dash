using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using DashLib.Models.Network;
using DashLib.Interfaces.Network;

namespace DashLib.Models.Monitoring
{
    public class PingState : BaseMonitorState, IMonitorState<PingState>
    {
        public PingState(BaseMonitoringTarget target) : base(target) { }
        public PingState(BaseMonitoringTarget target, DateTime timeStamp) : base(target, timeStamp) { }

        static public List<PingState> GetMonitorStatesFromListDns(List<DnsMonitoringTarget> dnsMonitoringTargets)
        {
            var states = dnsMonitoringTargets.SelectMany(x => x.IcmpMonitorStates).ToList();
            return states ?? [];
        }

        static public List<PingState> GetMonitorStatesFromListIp(List<IpMonitoringTarget> ipMonitoringTargets)
        {
            var states = ipMonitoringTargets.SelectMany(x => x.IcmpMonitorStates).ToList();
            return states ?? [];
        }

        static public List<PingState> GetMostRecentStatesFromListDns(List<DnsMonitoringTarget> dnsMonitoringTargets)
        {
            var state = dnsMonitoringTargets.SelectMany(x => x.IcmpMonitorStates)
                .OrderByDescending(x => x.Timestamp)
                .First();

            return [state];
        }

        static public List<PingState> GetMostRecentStatesFromListIp(List<IpMonitoringTarget> ipMonitoringTargets)
        {
            var state = ipMonitoringTargets.SelectMany(x => x.IcmpMonitorStates)
                .OrderByDescending(x => x.Timestamp)
                .First();

            return [state];
        }
    }
}
