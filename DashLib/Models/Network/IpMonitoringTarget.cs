using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using DashLib.Models.Monitoring;
using DashLib.Models.Dashboard;
using DashLib.Interfaces.Monitoring;
using System.Net.Sockets;

namespace DashLib.Models.Network
{
    public class IpMonitoringTarget : BaseMonitoringTarget, IMonitoringTarget
    {
        public IpMonitoringTarget() : base()
        {

        }

        public IpMonitoringTarget(BaseMonitoringTargetContainer parent) : base(parent)
        {
            Address = ConvertToByte("192.168.0.1");
            ParentId = parent.Id;
        }

        public IpMonitoringTarget(IpMonitoringTarget ip)
        {
            Id = ip.Id;
            ParentId = ip.ParentId;
            Parent = ip.Parent;
            Hostname = ip.Hostname;
            IsMonitoredIcmp = ip.IsMonitoredIcmp;
            IsMonitoredTcp = ip.IsMonitoredTcp;
            TcpPortsMonitored = ip.TcpPortsMonitored;
            TcpMonitorStates = ip.TcpMonitorStates;
            IcmpMonitorStates = ip.IcmpMonitorStates;
            Address = ip.Address;
        }

        public int ParentId { get; set; }
        public byte[] Address { get; set; }

        static public byte[] ConvertToByte(IPAddress ip)
        {
            return IPAddress.Parse(ip.ToString()).GetAddressBytes();
        }

        static public byte[] ConvertToByte(string ip)
        {
            return IPAddress.Parse(ip).GetAddressBytes();
        }

        static public byte[] GetMaskFromCidr(int cidr)
        {
            var mask = (cidr == 0) ? 0 : uint.MaxValue << (32 - cidr);
            return BitConverter.GetBytes(mask).Reverse().ToArray();
        }

        static public string ConvertToString(byte[] ip)
        {
            var temp = new IPAddress(ip);
            return temp.ToString();
        }

        static public IpMonitoringTarget Clone(IpMonitoringTarget ip)
        {
            var _ip = new IpMonitoringTarget(ip);

            return _ip;
        }

        public async Task<PingState> IcmpTestAsync()
        {
            var pingState = new PingState(this);
            using var ping = new Ping();
            try
            {
                pingState.Response = (ping.Send(new IPAddress(this.Address)).Status == IPStatus.Success);
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
            var address = new IPAddress(this.Address);
            var ts = DateTime.Now;

            foreach (var port in this.TcpPortsMonitored)
            {
                var state = new PortState(this, ts, port);

                try
                {
                    await client.ConnectAsync(address, port);
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