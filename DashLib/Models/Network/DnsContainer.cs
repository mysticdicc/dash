using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Network
{
    public class DnsContainer : BaseMonitoringTargetContainer
    {
        public string DisplayName { get; set; }
        public new List<DnsMonitoringTarget> Children { get; set; }
        public DnsContainer(string display) : base()
        {
            DisplayName = display;
            Children = [];
        }
    }
}
