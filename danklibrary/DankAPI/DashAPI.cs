using danklibrary.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace danklibrary.DankAPI
{
    public class DashAPI : IDashAPI
    {
        private readonly HttpClient _httpClient;
        private string _directoryBase;
        private string _shortcutBase;

        public DashAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _directoryBase = $"{httpClient.BaseAddress}dashboard/v2/directories";
            _shortcutBase = $"{httpClient.BaseAddress}dashboard/v2/shortcuts";
        }

        public async Task<List<DashboardItemBase>> GetAllItemsAsync()
        {
            List<DashboardItemBase>? items = [];
            List<ShortcutItem>? shortcutItems = [];
            List<DirectoryItem>? directoryItems = [];

            string shortcutEndpoint = $"{_shortcutBase}/get/noparent";
            string directoryEndpoint = $"{_directoryBase}/get/all";

            try
            {
                shortcutItems = await RequestHandler.GetFromJsonAsync<List<ShortcutItem>>(_httpClient, shortcutEndpoint);
            }
            catch
            {
                throw;
            }

            if (shortcutItems != null && shortcutItems.Count > 0)
            {
                items.AddRange(shortcutItems);
            }

            try
            {
                directoryItems = await RequestHandler.GetFromJsonAsync<List<DirectoryItem>>(_httpClient, directoryEndpoint);
            }
            catch
            {
                throw;
            }

            if (directoryItems != null && directoryItems.Count > 0)
            {
                items.AddRange(directoryItems);
            }

            if (items.Count > 0)
            {
                return items;
            }
            else
            {
                throw new InvalidOperationException("List is empty");
            }
        }

        public async Task<bool> DeleteItemAsync(DashboardItemBase item)
        {
            if (item is ShortcutItem shortcut)
            {
                var dto = ShortcutItemDto.FromEntity(shortcut);
                string endpoint = $"{_shortcutBase}/delete/byobject";

                try
                {
                    await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directory)
            {
                var dto = DirectoryItemDto.FromEntity(directory);
                string endpoint = $"{_directoryBase}/delete/byobject";

                try
                {
                    var response = await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new InvalidCastException("object is not directory item or shortcut item");
            }
        }

        public async Task<bool> SaveItemAsync(DashboardItemBase item)
        {
            if (item is ShortcutItem shortcutSend)
            {
                var dto = ShortcutItemDto.FromEntity(shortcutSend);
                string endpoint = $"{_shortcutBase}/delete/byobject";

                try
                {
                    var response = await RequestHandler.PostJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directorySend)
            {
                var dto = DirectoryItemDto.FromEntity(directorySend);
                string endpoint = $"{_directoryBase}/delete/byobject";

                try
                {
                    var response = await RequestHandler.PostJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new InvalidCastException("object is not directory item or shortcut item");
            }
        }

        public async Task<bool> EditItemAsync(DashboardItemBase item)
        {
            if (item is ShortcutItem shortcut)
            {
                var dto = ShortcutItemDto.FromEntity(shortcut);
                string endpoint = $"{_shortcutBase}/put/update";

                try
                {
                    await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directory)
            {
                var dto = DirectoryItemDto.FromEntity(directory);
                string endpoint = $"{_directoryBase}/put/update";

                try
                {
                    await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, dto);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new InvalidCastException("object is not directory item or shortcut item");
            }
        }
    }
}
