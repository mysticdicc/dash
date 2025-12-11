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
    public class DashboardController(IDbContextFactory<danknetContext> dbContext) : Controller
    {
        private readonly IDbContextFactory<danknetContext> _DbFactory = dbContext;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

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
            items.AddRange(context.DirectoryItems.Include(x => x.Children).ToList());
            return JsonSerializer.Serialize(items, JsonOptions);
        }

        [HttpPost]
        [Route("[controller]/v2/shortcuts/post/new")]
        public async Task<Results<BadRequest<string>, Created<ShortcutItemDto>>> NewShortcut(ShortcutItemDto dto)
        {
            using var context = _DbFactory.CreateDbContext();
            var entity = new ShortcutItem
            {
                Id = dto.Id,
                DisplayName = dto.DisplayName ?? string.Empty,
                Description = dto.Description,
                Icon = dto.Icon,
                Url = dto.Url ?? string.Empty
            };

            if (dto.ParentId is Guid parentId)
            {
                var parent = await context.DirectoryItems.FindAsync(parentId);
                if (parent != null) entity.Parent = parent;
            }

            try
            {
                context.ShortcutItems.Add(entity);
                await context.SaveChangesAsync();
                return TypedResults.Created(entity.Id.ToString(), dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/directories/post/new")]
        public async Task<Results<BadRequest<string>, Created<DirectoryItemDto>>> NewDirectory(DirectoryItemDto dto)
        {
            using var ctx = _DbFactory.CreateDbContext();
            var entity = new DirectoryItem
            {
                Id = dto.Id,
                DisplayName = dto.DisplayName ?? string.Empty,
                Description = dto.Description,
                Icon = dto.Icon,
                Children = []
            };

            try
            {
                ctx.DirectoryItems.Add(entity);
                await ctx.SaveChangesAsync();
                return TypedResults.Created(entity.Id.ToString(), dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/shortcuts/put/update")]
        public async Task<Results<BadRequest<string>, Ok<ShortcutItemDto>>> UpdateShortcut(ShortcutItemDto dto)
        {
            using var context = _DbFactory.CreateDbContext();
            var entity = await context.ShortcutItems.Include(x => x.Parent)
                                                .FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity == null) return TypedResults.BadRequest("Bad ID match");

            dto.ApplyToEntity(entity);

            if (dto.ParentId is Guid parentId)
            {
                entity.Parent = await context.DirectoryItems.FindAsync(parentId);
            }
            else
            {
                entity.Parent = null;
            }

            try
            {
                context.ShortcutItems.Update(entity);
                await context.SaveChangesAsync();
                return TypedResults.Ok(dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/directories/put/update")]
        public async Task<Results<BadRequest<string>, Ok<DirectoryItemDto>>> UpdateDirectory(DirectoryItemDto dto)
        {
            using var context = _DbFactory.CreateDbContext();
            var entity = await context.DirectoryItems.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity == null) return TypedResults.BadRequest("Bad ID match");

            dto.ApplyToEntity(entity);

            try
            {
                context.DirectoryItems.Update(entity);
                await context.SaveChangesAsync();
                return TypedResults.Ok(dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/shortcuts/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<ShortcutItemDto>>> DeleteShortcut(ShortcutItemDto dto)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = await context.ShortcutItems.Include(x => x.Parent).FirstOrDefaultAsync(x => x.Id == dto.Id);

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
                return TypedResults.Ok(dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/directories/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<DirectoryItemDto>>> DeleteDirectory(DirectoryItemDto dto)
        {
            using var context = _DbFactory.CreateDbContext();
            var delete = await context.DirectoryItems.Include(d => d.Children).FirstOrDefaultAsync(x => x.Id == dto.Id);

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
                return TypedResults.Ok(dto);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"{ex.Message}");
            }
        }
    }
}
