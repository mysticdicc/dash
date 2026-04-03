using DashLib.Models.Monitoring.Arr;
using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.DTO.Arr
{
    public class DiskSizeResponseDto
    {
        //https://sonarr.tv/docs/api/#v3/tag/diskspace/GET/api/v3/diskspace
        public Int32 Id { get; set; }
        public string? Path { get; set; }
        public string? Label { get; set; }
        public Int64 FreeSpace { get; set; }
        public Int64 TotalSpace { get; set; }

        public ArrDiskSizeMonitorState ToMonitorState(ArrMonitoringTarget target)
        {
            var state = new ArrDiskSizeMonitorState(target);
            state.Path = Path ?? string.Empty;
            state.Label = Label ?? string.Empty;
            state.FreeSpace = FreeSpace;
            state.TotalSpace = TotalSpace;
            return state;
        }
    }
}
