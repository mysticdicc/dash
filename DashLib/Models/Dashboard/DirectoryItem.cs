using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Models.Dashboard
{
    public class DirectoryItem : DashboardItemBase
    {
        public override DashboardItemType Type => DashboardItemType.Directory;
        public List<ShortcutItem> Children { get; set; } = [];
        public string Icon { get; set; } = string.Empty;
    }
}
