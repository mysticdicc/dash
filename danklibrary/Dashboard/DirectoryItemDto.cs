using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace danklibrary.Dashboard
{
    public sealed class DirectoryItemDto
    {
        public Guid Id { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }

        public static DirectoryItemDto FromEntity(DirectoryItem entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            return new DirectoryItemDto
            {
                Id = entity.Id,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Icon = entity.Icon
            };
        }

        public void ApplyToEntity(DirectoryItem entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            entity.DisplayName = DisplayName;
            entity.Description = Description;
            entity.Icon = Icon;
        }
    }
}
