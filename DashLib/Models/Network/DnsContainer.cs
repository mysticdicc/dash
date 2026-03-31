using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models.Network
{
    public class DnsContainer : BaseMonitoringTargetContainer
    {
        public string DisplayName { get; set; }
        public new List<DnsMonitoringTarget> Children { get; set; }
        [JsonConstructor] public DnsContainer() { }
        public DnsContainer(string display) : base()
        {
            DisplayName = display;
            Children = [];
        }
    }
}
