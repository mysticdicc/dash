using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Models.Monitoring.Arr
{
    public class ArrDiskSizeMonitorState : BaseMonitorState
    {
        public string Path { get; set; }
        public string Label { get; set; }
        public Int64 FreeSpace { get; set; }
        public Int64 TotalSpace { get; set; }

        public ArrDiskSizeMonitorState(ArrMonitoringTarget target) : base()
        {
            Target = target;
            TargetId = target.Id;
            Path = string.Empty;
            Label = string.Empty;
            FreeSpace = 0;
            TotalSpace = 0;
        }

        public ArrDiskSizeMonitorState(ArrMonitoringTarget target, DateTime timeStamp) : base(target, timeStamp)
        {
            Target = target;
            Path = string.Empty;
            Label = string.Empty;
            FreeSpace = 0;
            TotalSpace = 0;
        }
    }
}
