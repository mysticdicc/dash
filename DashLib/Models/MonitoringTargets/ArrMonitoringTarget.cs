using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Monitoring.Arr;
using DashLib.Models.Network;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace DashLib.Models.MonitoringTargets
{
    public enum ArrAddressType
    {
        IpAddress,
        DnsAddress
    }

    public abstract class ArrMonitoringTarget : BaseMonitoringTarget, IMonitoringTarget
    {
        public int ParentId { get; set; }
          
        public ArrAddressType AddressType { get; set; } = ArrAddressType.DnsAddress;

        private byte[]? _ipAddress;
        private string? _dnsAddress;
        public bool IsDiskSpaceMonitored { get; set; }
        public bool IsHealthStateMonitored { get; set; }
        public virtual List<ArrDiskSizeMonitorState> DiskSizeStates { get; set; } = [];
        public virtual List<ArrHealthMonitorState> HealthStates { get; set; } = [];
        public string ApiKey { get; set; } = string.Empty;
        public int Port { get; set; } = 7878;
        public bool UseHttps { get; set; } = false;

        public void SetIpAddress(byte[] ipAddress)
        {
            AddressType = ArrAddressType.IpAddress;
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            _dnsAddress = null;
        }

        public void SetDnsAddress(string dnsAddress)
        {
            if (string.IsNullOrWhiteSpace(dnsAddress))
                throw new ArgumentException("DNS address cannot be empty", nameof(dnsAddress));

            AddressType = ArrAddressType.DnsAddress;
            _dnsAddress = dnsAddress;
            _ipAddress = null;
        }

        public T Match<T>(Func<byte[], T> onIp, Func<string, T> onDns)
        {
            return AddressType switch
            {
                ArrAddressType.IpAddress when _ipAddress != null => onIp(_ipAddress),
                ArrAddressType.DnsAddress when _dnsAddress != null => onDns(_dnsAddress),
                _ => throw new InvalidOperationException("Address not properly initialized")
            };
        }

        public void Match(Action<byte[]> onIp, Action<string> onDns)
        {
            switch (AddressType)
            {
                case ArrAddressType.IpAddress when _ipAddress != null:
                    onIp(_ipAddress);
                    break;
                case ArrAddressType.DnsAddress when _dnsAddress != null:
                    onDns(_dnsAddress);
                    break;
                default:
                    throw new InvalidOperationException("Address not properly initialized");
            }
        }
        public string GetEffectiveAddress()
        {
            return Match(
                onIp: ip => new IPAddress(ip).ToString(),
                onDns: dns => dns
            );
        }

        public string GetApiUrl(string endpoint = "")
        {
            var protocol = UseHttps ? "https" : "http";
            var address = GetEffectiveAddress();
            var baseUrl = $"{protocol}://{address}:{Port}";
            return string.IsNullOrEmpty(endpoint) ? baseUrl : $"{baseUrl}/{endpoint.TrimStart('/')}";
        }

        public byte[]? IpAddress
        {
            get => _ipAddress;
            set
            {
                if (value != null && AddressType == ArrAddressType.IpAddress)
                    _ipAddress = value;
            }
        }

        public string? DnsAddress
        {
            get => _dnsAddress;
            set
            {
                if (value != null && AddressType == ArrAddressType.DnsAddress)
                    _dnsAddress = value;
            }
        }

        public bool IsValid()
        {
            return AddressType switch
            {
                ArrAddressType.IpAddress => _ipAddress != null && _ipAddress.Length > 0,
                ArrAddressType.DnsAddress => !string.IsNullOrWhiteSpace(_dnsAddress),
                _ => false
            };
        }

        public async Task<PingState> IcmpTestAsync()
        {
            var pingState = new PingState(this);
            using var ping = new Ping();
            try
            {
                this.Match(onIp: ipBytes =>
                {
                    pingState.Response = (ping.Send(new IPAddress(ipBytes)).Status == IPStatus.Success);
                },
                onDns: dnsAddr =>
                {
                    pingState.Response = (ping.Send(dnsAddr).Status == IPStatus.Success);
                });
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
                    await this.Match(onIp: async ipBytes =>
                    {
                        var address = new IPAddress(ipBytes);
                        await client.ConnectAsync(address, port);
                    },
                    onDns: async dnsAddr =>
                    {
                        await client.ConnectAsync(dnsAddr, port);
                    });
                    
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