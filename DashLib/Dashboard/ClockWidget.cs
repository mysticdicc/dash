using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    public class ClockWidget : WidgetItem
    {
        public override WidgetType TypeOfWidget => WidgetType.Clock;

        [SetsRequiredMembers]
        public ClockWidget()
        {
            this.Id = Guid.NewGuid();
            this.DisplayName = "Clock";
            this.Description = "A 24-hour clock widget.";
        }
    }
}
