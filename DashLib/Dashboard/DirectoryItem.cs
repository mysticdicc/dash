using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    public class DirectoryItem : DashboardItemBase
    {
        public override DashboardItemType Type => DashboardItemType.Directory;
        required public List<ShortcutItem> Children { get; set; }
    }
}
