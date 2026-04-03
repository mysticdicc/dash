using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.MonitoringTargetContainers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DashLib.Models.Network
{
    public class DnsMonitoringTarget : BaseMonitoringTarget, IMonitoringTarget
    {
        public DnsMonitoringTarget() : base()
        {

        }

        public DnsMonitoringTarget(BaseMonitoringTargetContainer parent) : base(parent)
        {
            Address = string.Empty;
            ParentId = parent.Id;
        }

        public int ParentId { get; set; }
        public string Address { get; set; }

        public async Task<PingState> IcmpTestAsync()
        {
            var pingState = new PingState(this);
            using var ping = new Ping();
            try
            {
                pingState.Response = (ping.Send(this.Address).Status == IPStatus.Success);
            }
            catch
            {
                pingState.Response = false;
            }
            return pingState;
        }

        public async Task<List<PortState>> TcpTestAsync()
        {
            using var client = new TcpClient();
            var list = new List<PortState>();
            var ts = DateTime.Now;

            foreach (var port in this.TcpPortsMonitored)
            {
                var state = new PortState(this, ts, port);

                try
                {
                    await client.ConnectAsync(this.Address, port);
                    state.Response = true;
                }
                catch
                {
                    state.Response = false;
                }

                list.Add(state);
            }

            return list;
        }
    }
}
