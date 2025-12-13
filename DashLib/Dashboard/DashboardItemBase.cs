using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DashLib.Dashboard
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "DashItemType")]
    [JsonDerivedType(typeof(ShortcutItem), "Shortcut")]
    [JsonDerivedType(typeof(DirectoryItem), "Directory")]
    [JsonDerivedType(typeof(ClockWidget), "ClockWidget")]
    [JsonDerivedType(typeof(DeviceStatusWidget), "DeviceStatusWidget")]
    public abstract class DashboardItemBase
    {
        required public Guid Id { get; set; }
        required public string DisplayName { get; set; }
        public string? Description { get; set; }
        public enum DashboardItemType
        {
            Shortcut,
            Directory,
            Widget
        }
        abstract public DashboardItemType Type { get; }
    }
}
