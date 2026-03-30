using DashLib.Interfaces.Monitoring;
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

        static public List<PortState> GetAllMonitorStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            var states = baseTargets.SelectMany(x => x.TcpMonitorStates).ToList();
            return states;
        }

        static public List<PortState> GetMostRecentStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets)
        {
            return baseTargets
                .SelectMany(t => t.TcpMonitorStates)
                .GroupBy(s => (s.TargetId, s.TargetPort))
                .Select(g => g.MaxBy(x => x.Timestamp)!)
                .Where(s => s != null)
                .ToList();
        }

        public static List<PortState> GetAllMonitorStatesFromMonitoringTarget(BaseMonitoringTarget target)
        {
            return target.TcpMonitorStates.ToList();
        }

        public static List<PortState> GetMostRecentStateFromMonitoringTarget(BaseMonitoringTarget target)
        {
            return target.TcpMonitorStates
                .GroupBy(x => (x.TargetId, x.TargetPort))
                .Select(x => x.MaxBy(x => x.Timestamp)!)
                .Where(x => x != null)
                .ToList();
        }

        public static float CalculateUptimePercentage(TimeSpan timeFromNow, List<PortState> states)
        {
            var oldDate = DateTime.UtcNow - timeFromNow;

            var portStates = states
                .Where(x => x.Timestamp > oldDate)
                .OrderBy(x => x.Timestamp)
                .ToList();

            int totalCount = portStates.Count();
            int upCount = portStates.Where(x => x.Response == true).ToList().Count();

            if (totalCount <= 0)
            {
                totalCount = 1;
            }

            float uptimePercent = (upCount / totalCount) * 100;
            return uptimePercent;
        }
    }
}
