using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "WidgetIsType")]
    [JsonDerivedType(typeof(ClockWidget), "ClockWidget")]
    [JsonDerivedType(typeof(DeviceStatusWidget), "DeviceStatusWidget")]
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
