using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Network
{
    public interface IMonitorState<TState> where TState : BaseMonitorState
    {
        static abstract public List<TState> GetMonitorStatesFromListIp(List<IpMonitoringTarget> ipMonitoringTargets);
        static abstract public List<TState> GetMonitorStatesFromListDns(List<DnsMonitoringTarget> dnsMonitoringTargets);
        static abstract public List<TState> GetMostRecentStatesFromListIp(List<IpMonitoringTarget> ipMonitoringTargets);
        static abstract public List<TState> GetMostRecentStatesFromListDns(List<DnsMonitoringTarget> dnsMonitoringTargets);
    }
}
