using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashLib.Network;

namespace DashLib.Dashboard
{
    public class DeviceStatusWidget : WidgetItem
    {
        public override WidgetType TypeOfWidget => WidgetType.DeviceStatus;
        public int IP_ID { get; set; }

        //ef mapping
        public virtual IP? IP { get; set; }

        [SetsRequiredMembers]
        public DeviceStatusWidget()
        {
            this.Id = Guid.NewGuid();
            this.DisplayName = "Device Status";
            this.Description = "A widget that displays the status of a network device.";
        }
    }
}
