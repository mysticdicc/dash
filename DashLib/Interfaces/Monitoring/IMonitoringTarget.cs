using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Monitoring
{
    public interface IMonitoringTarget
    {
        public Task<PingState> IcmpTestAsync();
        public Task<List<PortState>> TcpTestAsync();
    }
}
