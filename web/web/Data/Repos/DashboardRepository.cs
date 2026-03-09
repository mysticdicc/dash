using dankweb.API;
using DashLib.Interfaces.Dashboard;
using DashLib.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace web.Data.Repos
{
    public class DashboardRepository(IDbContextFactory<DashDbContext> dbContext) : IDashboardRepository
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        public async Task<bool> AddClockWidgetAsync(ClockWidget widget)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.ClockWidgets.AddAsync(widget);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> AddDirectoryAsync(DirectoryItem directory)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.DirectoryItems.AddAsync(directory);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> AddShortcutAsync(ShortcutItem shortcut)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            await ctx.ShortcutItems.AddAsync(shortcut);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> AddStatusWidgetAsync(DeviceStatusWidget widget)
        {
            using var ctx = _dbFactory.CreateDbContext();
            await ctx.DeviceStatusWidgets.AddAsync(widget);
            var rows = await ctx.SaveChangesAsync();

            return rows > 0;
        }

        public async Task<bool> DeleteClockWidgetAsync(ClockWidget widget)
        {
            using var ctx = _dbFactory.CreateDbContext();
            var entity = await ctx.ClockWidgets.FirstOrDefaultAsync(x => x.Id == widget.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {widget.Id}");

            ctx.ClockWidgets.Remove(entity);
            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteDirectoryAsync(DirectoryItem directory)
        {
            using var ctx = _dbFactory.CreateDbContext();
            var entity = await ctx.DirectoryItems.FirstOrDefaultAsync(x => x.Id == directory.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {directory.Id}");

            ctx.DirectoryItems.Remove(entity);
            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteShortcutAsync(ShortcutItem shortcut)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.ShortcutItems.FirstOrDefaultAsync(x => x.Id == shortcut.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {shortcut.Id}");

            ctx.ShortcutItems.Remove(entity);
            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteStatusWidgetAsync(DeviceStatusWidget widget)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DeviceStatusWidgets.FirstOrDefaultAsync(x => x.Id == widget.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {widget.Id}");

            ctx.DeviceStatusWidgets.Remove(entity);
            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<IReadOnlyList<DirectoryItem>> GetAllDirectoriesAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.DirectoryItems.ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<ShortcutItem>> GetAllShortcutsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.ShortcutItems.ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<ClockWidget>> GetClockWidgetsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.ClockWidgets.ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<ShortcutItem>> GetShortcutsWithNoParentAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.ShortcutItems.Where(x => x.Parent == null).ToListAsync();
            return list;
        }

        public async Task<IReadOnlyList<DeviceStatusWidget>> GetStatusWidgetsAsync()
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var list = await ctx.DeviceStatusWidgets.ToListAsync();
            return list;
        }

        public async Task<bool> UpdateDirectoryAsync(DirectoryItem directory)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.DirectoryItems.FirstOrDefaultAsync(x => x.Id == directory.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {directory.Id}");

            entity.DisplayName = directory.DisplayName;
            entity.Children = directory.Children;
            entity.Icon = directory.Icon;
            entity.Description = directory.Description;

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateShortcutAsync(ShortcutItem shortcut)
        {
            using var ctx = await _dbFactory.CreateDbContextAsync();
            var entity = await ctx.ShortcutItems.FirstOrDefaultAsync(x => x.Id == shortcut.Id);

            if (entity == null) throw new InvalidDataException($"No entity with ID: {shortcut.Id}");

            entity.DisplayName = shortcut.DisplayName;
            entity.Url = shortcut.Url;
            entity.ParentId = shortcut.ParentId;
            entity.Icon = shortcut.Icon;
            entity.Description = shortcut.Description;

            var rows = await ctx.SaveChangesAsync();
            return rows > 0;
        }
    }
}
