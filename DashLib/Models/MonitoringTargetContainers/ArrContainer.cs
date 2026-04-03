using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.MonitoringTargetContainers
{
    public class ArrContainer : BaseMonitoringTargetContainer<ArrMonitoringTarget>
    {
        public string DisplayName { get; set; }
        public override int ChildCount => Children.Count;

        public ArrContainer(string display) : base()
        {
            DisplayName = display;
            Children = [];
        }

    }
}
