using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace DashLib.Models.Monitoring
{
    public abstract class BaseMonitorState
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public int TargetId { get; set; }
        public virtual BaseMonitoringTarget Target { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Response { get; set; }

        public BaseMonitorState()
        {
            Id = 0;
            TargetId = 0;
            Target = new IpMonitoringTarget();
            Timestamp = DateTime.Now;
            Response = false;
        }

        public BaseMonitorState(BaseMonitoringTarget target)
        {
            Id = 0;
            TargetId = target.Id;
            Target = target;
            Timestamp = DateTime.Now;
            Response = false;
        } 

        public BaseMonitorState(BaseMonitoringTarget target, DateTime timeStamp)
        {
            Id = 0;
            TargetId = target.Id;
            Target = target;
            Timestamp = timeStamp;
            Response = false;
        }

        static public List<BaseMonitoringTarget> GetMonitorStatesFromTimespan(List<BaseMonitoringTarget> targets, TimeSpan length)
        {
            var oldDate = DateTime.Now - length;
            foreach (var target in targets)
            {
                target.TcpMonitorStates = target.TcpMonitorStates.Where(x => x.Timestamp > oldDate).ToList();
                target.IcmpMonitorStates = target.IcmpMonitorStates.Where(x => x.Timestamp > oldDate).ToList();
            }
            return targets;
        }
    }
}
