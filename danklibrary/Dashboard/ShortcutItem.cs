using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace danklibrary.Dashboard
{
    public class ShortcutItem : DashboardItemBase
    {
        public required string Url { get; set; }
        public override DashboardItemType Type => DashboardItemType.Shortcut;
        public DirectoryItem? Parent { get; set; }

        public static bool UrlIsValid(ShortcutItem item)
        {
            bool uriValid;

            if (null != item.Url && String.Empty != item.Url)
            {
                uriValid = Uri.TryCreate(item.Url, UriKind.Absolute, out Uri? uri)
                    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            }
            else
            {
                uriValid = false;
            }

            if (uriValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
