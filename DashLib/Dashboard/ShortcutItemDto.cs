using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Dashboard
{
    public sealed class ShortcutItemDto
    {
        public Guid Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public Guid? ParentId { get; set; }

        public static ShortcutItemDto FromEntity(ShortcutItem entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new ShortcutItemDto
            {
                Id = entity.Id,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Icon = entity.Icon,
                Url = entity.Url,
                ParentId = entity.Parent?.Id
            };
        }

        public void ApplyToEntity(ShortcutItem entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            entity.DisplayName = DisplayName ?? string.Empty;
            entity.Description = Description;
            entity.Icon = Icon;
            entity.Url = Url ?? string.Empty;
        }
    }
}
