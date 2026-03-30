using DashLib.Models.Monitoring;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DashLib.Models.Network
{
    public class DnsMonitoringTarget : BaseMonitoringTarget
    {
        public DnsMonitoringTarget() : base()
        {

        }

        public DnsMonitoringTarget(BaseMonitoringTargetContainer parent) : base(parent)
        {
            Parent = (DnsContainer)parent;
            Address = string.Empty;
        }

        new public DnsContainer Parent { get; set; }
        public string Address { get; set; }

        public override async Task<PingState> IcmpTestAsync(BaseMonitoringTarget target)
        {
            if (target is DnsMonitoringTarget dns)
            {
                var pingState = new PingState(dns);
                using var ping = new Ping();
                try
                {
                    pingState.Response = (ping.Send(dns.Address).Status == IPStatus.Success);
                }
                catch
                {
                    pingState.Response = false;
                }
                return pingState;
            }
            else
            {
                throw new InvalidCastException("DnsMonitoringTarget is required as argument.");
            }
        }

        public override async Task<List<PortState>> TcpTestAsync(BaseMonitoringTarget target)
        {
            if (target is DnsMonitoringTarget dns)
            {
                using var client = new TcpClient();
                var list = new List<PortState>();
                var ts = DateTime.Now;

                foreach (var port in dns.TcpPortsMonitored)
                {
                    var state = new PortState(dns, ts, port);

                    try
                    {
                        await client.ConnectAsync(dns.Address, port);
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
            else
            {
                throw new InvalidCastException("DnsMonitoringTarget is required as argument.");
            }
        }
    }
}
