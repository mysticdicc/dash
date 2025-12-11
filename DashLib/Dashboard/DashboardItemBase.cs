using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    public abstract class DashboardItemBase
    {
        required public Guid Id { get; set; }
        required public string DisplayName { get; set; }
        public string? Description { get; set; }
        public enum DashboardItemType
        {
            Shortcut,
            Directory
        }
        abstract public DashboardItemType Type { get; }
        public string? Icon { get; set; }
    }
}
