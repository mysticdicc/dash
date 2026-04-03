using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models.MonitoringTargetContainers
{
    public class DnsContainer : BaseMonitoringTargetContainer<DnsMonitoringTarget>
    {
        public string DisplayName { get; set; }
        public override int ChildCount => Children.Count;
        [JsonConstructor] public DnsContainer() { }
        public DnsContainer(string display) : base()
        {
            DisplayName = display;
            Children = [];
        }
    }
}
