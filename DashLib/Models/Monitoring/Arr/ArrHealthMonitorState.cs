using DashLib.DTO.Arr;
using DashLib.Models.MonitoringTargets;
using System;
using System.Collections.Generic;
using System.Text;
using static DashLib.DTO.Arr.HealthResponseDto;

namespace DashLib.Models.Monitoring.Arr
{
    public class ArrHealthMonitorState : BaseMonitorState
    {
        public string Source { get; set; }
        public HealthType Type { get; set; }
        public string Message { get; set; }
        public string FullUri { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }
       
        public ArrHealthMonitorState(ArrMonitoringTarget target) : base()
        {
            TargetId = target.Id;
            Target = target;
            Source = string.Empty;
            Message = string.Empty;
            FullUri = string.Empty;
            Scheme = string.Empty;
            Host = string.Empty;
            Port = string.Empty;
            Path = string.Empty;
            Query = string.Empty;
            Fragment = string.Empty;
        }
    }
}
