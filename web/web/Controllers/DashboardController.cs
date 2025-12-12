using DashLib.Dashboard;
using dankweb.API;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;

namespace web.Controllers
{
    [ApiController]
    public class DashboardController(IDbContextFactory<DashDbContext> dbContext) : Controller
    {
        private readonly IDbContextFactory<DashDbContext> _DbFactory = dbContext;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        [HttpGet]
        [Route("[controller]/v2/widgets/status/get/all")]
        public string GetAllStatusWidgets()
        {
            using var context = _DbFactory.CreateDbContext();
            return JsonSerializer.Serialize(context.DeviceStatusWidgets.ToList(), JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/widgets/clocks/get/all")]
        public string GetAllWidgets()
        {
            using var context = _DbFactory.CreateDbContext();
            return JsonSerializer.Serialize(context.ClockWidgets.ToList(), JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/shortcuts/get/all")]
        public string GetAllShortcuts()
        {
            using var context = _DbFactory.CreateDbContext();
            List<ShortcutItem> items = context.ShortcutItems.ToList();
            return JsonSerializer.Serialize(items, JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/shortcuts/get/noparent")]
        public string GetShortcutsWithoutParent()
        {
            using var context = _DbFactory.CreateDbContext();
            List<ShortcutItem> items = [];
            items.AddRange([.. context.ShortcutItems.Where(x => x.Parent == null)]);
            return JsonSerializer.Serialize(items, JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/directories/get/all")]
        public string GetAllDirectories()
        {
            using var context = _DbFactory.CreateDbContext();
            List<DirectoryItem> items = [];
            items.AddRange(context.DirectoryItems.ToList());
            return JsonSerializer.Serialize(items, JsonOptions);
        }

        [HttpPost]
        [Route("[controller]/v2/shortcuts/post/new")]
        public async Task<Results<BadRequest<string>, Created<ShortcutItem>>> NewShortcut(ShortcutItem item)
        {
            using var context = _DbFactory.CreateDbContext();
            try
            {
                context.ShortcutItems.Add(item);
                await context.SaveChangesAsync();
                return TypedResults.Created(item.Id.ToString(), item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/widgets/clocks/post/new")]
        public async Task<Results<BadRequest<string>, Created<ClockWidget>>> NewClockWidget(ClockWidget item)
        {
            using var context = _DbFactory.CreateDbContext();
            try
            {
                context.ClockWidgets.Add(item);
                await context.SaveChangesAsync();
                return TypedResults.Created(item.Id.ToString(), item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/widgets/status/post/new")]
        public async Task<Results<BadRequest<string>, Created<DeviceStatusWidget>>> NewDeviceStatuskWidget(DeviceStatusWidget item)
        {
            using var context = _DbFactory.CreateDbContext();
            try
            {
                context.DeviceStatusWidgets.Add(item);
                await context.SaveChangesAsync();
                return TypedResults.Created(item.Id.ToString(), item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/directories/post/new")]
        public async Task<Results<BadRequest<string>, Created<DirectoryItem>>> NewDirectory(DirectoryItem item)
        {
            using var ctx = _DbFactory.CreateDbContext();

            try
            {
                ctx.DirectoryItems.Add(item);
                await ctx.SaveChangesAsync();
                return TypedResults.Created(item.Id.ToString(), item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/shortcuts/put/update")]
        public async Task<Results<BadRequest<string>, Ok<ShortcutItem>>> UpdateShortcut(ShortcutItem item)
        {
            using var context = _DbFactory.CreateDbContext();
            var entity = await context.ShortcutItems.Include(x => x.Parent)
                                                .FirstOrDefaultAsync(x => x.Id == item.Id);
            if (entity == null) return TypedResults.BadRequest("Bad ID match");

            entity.DisplayName = item.DisplayName;
            entity.Url = item.Url;
            entity.ParentId = item.ParentId;
            entity.Icon = item.Icon;
            entity.Description = item.Description;

            try
            {
                context.ShortcutItems.Update(entity);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/directories/put/update")]
        public async Task<Results<BadRequest<string>, Ok<DirectoryItem>>> UpdateDirectory(DirectoryItem item)
        {
            using var context = _DbFactory.CreateDbContext();
            var entity = await context.DirectoryItems.FirstOrDefaultAsync(x => x.Id == item.Id);
            if (entity == null) return TypedResults.BadRequest("Bad ID match");

            entity.DisplayName = item.DisplayName;
            entity.Children = item.Children;
            entity.Icon = item.Icon;
            entity.Description = item.Description;

            try
            {
                context.DirectoryItems.Update(entity);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/widgets/clocks/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<ClockWidget>>> DeleteClockWidget(ClockWidget item)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = context.ClockWidgets.Find(item.Id);

            if (delete == null)
            {
                return TypedResults.BadRequest("Unable to track db object");
            }

            try
            {
                context.ClockWidgets.Remove(delete);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/widgets/status/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<DeviceStatusWidget>>> DeleteStatusWidget(DeviceStatusWidget item)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = context.DeviceStatusWidgets.Find(item.Id);

            if (delete == null)
            {
                return TypedResults.BadRequest("Unable to track db object");
            }

            try
            {
                context.DeviceStatusWidgets.Remove(delete);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/shortcuts/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<ShortcutItem>>> DeleteShortcut(ShortcutItem item)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = await context.ShortcutItems.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == item.Id);

            if (delete == null)
            {
                return TypedResults.BadRequest("Unable to track db object");
            }

            if (delete.Parent != null)
            {
                var parent = await context.DirectoryItems.Include(d => d.Children).FirstOrDefaultAsync(d => d.Id == delete.Parent.Id);
                if (parent != null)
                {
                    parent.Children.Remove(delete);
                }
            }

            try
            {
                context.ShortcutItems.Remove(delete);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/directories/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<DirectoryItem>>> DeleteDirectory(DirectoryItem item)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = await context.DirectoryItems.Include(d => d.Children).FirstOrDefaultAsync(x => x.Id == item.Id);

            if (delete == null)
            {
                return TypedResults.BadRequest("Unable to track db object");
            }

            try
            {
                foreach (var child in delete.Children)
                {
                    context.ShortcutItems.Remove(child);
                }

                context.DirectoryItems.Remove(delete);
                await context.SaveChangesAsync();
                return TypedResults.Ok(item);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }
    }
}
