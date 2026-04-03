using DashLib.DTO.Arr;
using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Arr
{
    public interface IArrAPI
    {
        public Task<DiskSizeResponseDto> GetCurrentDiskSpaceAsync(ArrMonitoringTarget target);
        public Task<HealthResponseDto> GetCurrentHealthStatusAsync(ArrMonitoringTarget target);
    }
}
