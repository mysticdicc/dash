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
        public PingState() : base () { }
        public PingState(BaseMonitoringTarget target) : base(target) { }
        public PingState(BaseMonitoringTarget target, DateTime timeStamp) : base(target, timeStamp) { }

        static public List<PingState> GetAllMonitorStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            var states = baseTargets.SelectMany(x => x.IcmpMonitorStates).ToList();
            return states;
        }

        public static List<PingState> GetAllMonitorStatesFromMonitoringTarget(BaseMonitoringTarget target)
        {
            return target.IcmpMonitorStates.ToList();
        }

        public static List<PingState> GetMostRecentStateFromMonitoringTarget(BaseMonitoringTarget target)
        {
            return target.IcmpMonitorStates
                .GroupBy(x => x.TargetId)
                .Select(x => x.MaxBy(x => x.Timestamp)!)
                .ToList();
        }

        static public List<PingState> GetMostRecentStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            return baseTargets.SelectMany(x => x.IcmpMonitorStates)
                .GroupBy(x => x.TargetId)
                .Select(x => x.MaxBy(x => x.Timestamp)!)
                .ToList();
        }

        static public float CalculateUptimePercentage(TimeSpan timeFromNow, List<PingState> states)
        {
            var oldDate = DateTime.UtcNow - timeFromNow;

            var pingStates = states
                .Where(x => x.Timestamp > oldDate)
                .OrderBy(x => x.Timestamp)
                .ToList();

            int totalCount = pingStates.Count();
            int upCount = pingStates.Where(x => x.Response == true).ToList().Count();

            if (totalCount <= 0)
            {
                totalCount = 1;
            }

            float uptimePercent = (upCount / totalCount) * 100;
            return uptimePercent;
        }
    }
}
