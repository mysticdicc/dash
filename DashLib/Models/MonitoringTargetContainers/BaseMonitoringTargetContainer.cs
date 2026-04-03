using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DashLib.Models.MonitoringTargetContainers
{
    public abstract class BaseMonitoringTargetContainer<TChild> : BaseMonitoringTargetContainer
        where TChild : BaseMonitoringTarget
    {
        public List<TChild> Children { get; set; }
        public override IEnumerable<BaseMonitoringTarget> GetChildren() => Children;
        public override void AddChild(BaseMonitoringTarget child) => Children.Add((TChild)child);
        public override bool RemoveChild(BaseMonitoringTarget child) => Children.Remove((TChild)child);

        public BaseMonitoringTargetContainer()
        {
            Id = 0;
            Children = [];
        }
    }

    public abstract class BaseMonitoringTargetContainer
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public abstract IEnumerable<BaseMonitoringTarget> GetChildren();
        public abstract void AddChild(BaseMonitoringTarget child);
        public abstract bool RemoveChild(BaseMonitoringTarget child);
        public abstract int ChildCount { get; }

        public BaseMonitoringTargetContainer()
        {
            Id = 0;
        }
    }
}
