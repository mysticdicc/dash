using dankweb.API;
using DashLib.Dashboard;
using DashLib.Interfaces.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class DashboardRepository(IDbContextFactory<DashDbContext> dbContext) : IDashboardRepository
    {
        private readonly IDbContextFactory<DashDbContext> _DbFactory = dbContext;

        public Task<bool> AddClockWidgetAsync(ClockWidget widget)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddDirectoryAsync(DirectoryItem directory)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddShortcutAsync(ShortcutItem shortcut)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddStatusWidgetAsync(DeviceStatusWidget widget)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteClockWidgetAsync(ClockWidget widget)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDirectoryAsync(DirectoryItem directory)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteShortcutAsync(ShortcutItem shortcut)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteStatusWidgetAsync(DeviceStatusWidget widget)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DirectoryItem>> GetAllDirectoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ShortcutItem>> GetAllShortcutsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ClockWidget>> GetClockWidgetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ShortcutItem>> GetShortcutsWithNoParentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DeviceStatusWidget>> GetStatusWidgetsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateDirectoryAsync(DirectoryItem directory)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateShortcutAsync(ShortcutItem shortcut)
        {
            throw new NotImplementedException();
        }
    }
}
