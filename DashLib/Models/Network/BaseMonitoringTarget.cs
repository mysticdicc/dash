using DashLib.Interfaces.Monitoring;
using DashLib.Models.Dashboard;
using DashLib.Models.Monitoring;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models.Network
{
    public abstract class BaseMonitoringTarget
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int ParentId { get; set; }
        public virtual BaseMonitoringTargetContainer? Parent { get; set;  }
        public string Hostname { get; set; }
        public bool IsMonitoredIcmp { get; set; }
        public bool IsMonitoredTcp { get; set; }
        public List<int> TcpPortsMonitored { get; set; }
        public virtual List<PortState> TcpMonitorStates { get; set; }
        public virtual List<PingState> IcmpMonitorStates { get; set; }
        [JsonIgnore] public virtual DeviceStatusWidget? DeviceStatusWidget { get; }

        public BaseMonitoringTarget()
        {
            Id = 0;
            Parent = null;
            Hostname = string.Empty;
            IsMonitoredIcmp = false;
            IsMonitoredTcp = false;
            TcpPortsMonitored = [];
            TcpMonitorStates = [];
            IcmpMonitorStates = [];
        }

        public BaseMonitoringTarget(BaseMonitoringTargetContainer parent)
        {
            Id = 0;
            Parent = parent;
            Hostname = string.Empty;
            IsMonitoredIcmp = false;
            IsMonitoredTcp = false;
            TcpPortsMonitored = [];
            TcpMonitorStates = [];
            IcmpMonitorStates = [];
        }
    }
}
