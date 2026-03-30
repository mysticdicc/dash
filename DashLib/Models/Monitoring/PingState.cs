using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using DashLib.Models.Network;
using DashLib.Interfaces.Monitoring;

namespace DashLib.Models.Monitoring
{
    public class PingState : BaseMonitorState, IMonitorState<PingState>
    {
        public PingState(BaseMonitoringTarget target) : base(target) { }
        public PingState(BaseMonitoringTarget target, DateTime timeStamp) : base(target, timeStamp) { }

        static public List<PingState> GetAllMonitorStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            var states = baseTargets.SelectMany(x => x.IcmpMonitorStates).ToList();
            return states ?? [];
        }

        static public List<PingState> GetMostRecentStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            return baseTargets.SelectMany(x => x.IcmpMonitorStates)
                .GroupBy(x => x.Id)
                .Select(x => x.MaxBy(x => x.Timestamp))
                .Where(x => x != null)
                .ToList()!;
        }
    }
}
