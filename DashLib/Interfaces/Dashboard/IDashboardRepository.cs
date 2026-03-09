using DashLib.Models.Dashboard;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Dashboard
{
    public interface IDashboardRepository
    {
        public Task<IReadOnlyList<DeviceStatusWidget>> GetStatusWidgetsAsync();
        public Task<IReadOnlyList<ClockWidget>> GetClockWidgetsAsync();
        public Task<IReadOnlyList<ShortcutItem>> GetAllShortcutsAsync();
        public Task<IReadOnlyList<ShortcutItem>> GetShortcutsWithNoParentAsync();
        public Task<IReadOnlyList<DirectoryItem>> GetAllDirectoriesAsync();
        public Task<bool> AddShortcutAsync(ShortcutItem shortcut);
        public Task<bool> AddClockWidgetAsync(ClockWidget widget);
        public Task<bool> AddStatusWidgetAsync(DeviceStatusWidget widget);
        public Task<bool> AddDirectoryAsync(DirectoryItem directory);
        public Task<bool> UpdateShortcutAsync(ShortcutItem shortcut);
        public Task<bool> UpdateDirectoryAsync(DirectoryItem directory);
        public Task<bool> DeleteClockWidgetAsync(ClockWidget widget);
        public Task<bool> DeleteStatusWidgetAsync(DeviceStatusWidget widget);
        public Task<bool> DeleteShortcutAsync(ShortcutItem shortcut);
        public Task<bool> DeleteDirectoryAsync(DirectoryItem directory);
    }
}
