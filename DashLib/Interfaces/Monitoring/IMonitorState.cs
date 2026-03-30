using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitorState<TState> where TState : BaseMonitorState
    {
        static abstract public List<TState> GetAllMonitorStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets);
        static abstract public List<TState> GetAllMonitorStatesFromMonitoringTarget(BaseMonitoringTarget target);
        static abstract public List<TState> GetMostRecentStatesFromListMonitoringTargets(List<BaseMonitoringTarget> baseTargets);
        static abstract public List<TState> GetMostRecentStateFromMonitoringTarget(BaseMonitoringTarget target);
        static abstract public float CalculateUptimePercentage(TimeSpan timeFromNow, List<TState> states);
    }
}
