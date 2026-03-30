using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DashLib.Models.Network
{
    public abstract class BaseMonitoringTargetContainer
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int Id { get; set; }
        public List<BaseMonitoringTarget> Children { get; set; }

        public BaseMonitoringTargetContainer()
        {
            Id = 0;
            Children = [];
        }
    }
}
