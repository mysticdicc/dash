using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    public abstract class WidgetItem : DashboardItemBase
    {
        public override DashboardItemType Type => DashboardItemType.Widget;

        public enum WidgetType
        {
            Clock,
            DeviceStatus
        }

        public abstract WidgetType TypeOfWidget { get; }
    }
}
