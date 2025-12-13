using DashLib.Dashboard;
using DashLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.DankAPI
{
    public class DashAPI : IDashAPI
    {
        private readonly HttpClient _httpClient;
        private string _directoryBase;
        private string _shortcutBase;
        private string _widgetBase;

        public DashAPI(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _directoryBase = $"{httpClient.BaseAddress}dashboard/v2/directories";
            _shortcutBase = $"{httpClient.BaseAddress}dashboard/v2/shortcuts";
            _widgetBase = $"{httpClient.BaseAddress}dashboard/v2/widgets";
        }

        public async Task<List<DashboardItemBase>> GetAllItemsAsync()
        {
            List<DashboardItemBase>? items = [];

            List<ShortcutItem>? shortcutItems = [];
            List<DirectoryItem>? directoryItems = [];
            List<ClockWidget>? clockItems = [];
            List<DeviceStatusWidget>? statusItems = [];

            string shortcutEndpoint = $"{_shortcutBase}/get/all";
            string directoryEndpoint = $"{_directoryBase}/get/all";
            string clockEndpoint = $"{_widgetBase}/clocks/get/all";
            string statusEndpoint = $"{_widgetBase}/status/get/all";

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

            try
            {
                clockItems = await RequestHandler.GetFromJsonAsync<List<ClockWidget>>(_httpClient, clockEndpoint);
            }
            catch
            {
                throw;
            }

            if (clockItems != null && clockItems.Count > 0)
            {
                items.AddRange(clockItems);
            }

            try
            {
                statusItems = await RequestHandler.GetFromJsonAsync<List<DeviceStatusWidget>>(_httpClient, statusEndpoint);
            }
            catch
            {
                throw;
            }

            if (statusItems != null && statusItems.Count > 0)
            {
                items.AddRange(statusItems);
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
                string endpoint = $"{_shortcutBase}/delete/byobject";

                try
                {
                    await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, shortcut);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directory)
            {
                string endpoint = $"{_directoryBase}/delete/byobject";

                try
                {
                    await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, directory);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is WidgetItem widget)
            {
                if (widget.TypeOfWidget == WidgetItem.WidgetType.Clock)
                {
                    string endpoint = $"{_widgetBase}/clocks/delete/byobject";

                    try
                    {
                        await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, widget);
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else if (widget.TypeOfWidget == WidgetItem.WidgetType.DeviceStatus)
                {
                    string endpoint = $"{_widgetBase}/status/delete/byobject";

                    try
                    {
                        await RequestHandler.DeleteAsJsonAsync(_httpClient, endpoint, widget);
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw new InvalidCastException("object is not known widget type");
                }


            }
            else
            {
                throw new InvalidCastException("object is not known item type");
            }
        }

        public async Task<bool> SaveItemAsync(DashboardItemBase item)
        {
            if (item is ShortcutItem shortcutSend)
            {
                string endpoint = $"{_shortcutBase}/post/new";

                try
                {
                    await RequestHandler.PostJsonAsync(_httpClient, endpoint, shortcutSend);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directorySend)
            {
                string endpoint = $"{_directoryBase}/post/new";

                try
                {
                    await RequestHandler.PostJsonAsync(_httpClient, endpoint, directorySend);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is WidgetItem widgetSend)
            {
                string endpoint = $"{_widgetBase}/post/new";

                if (widgetSend.TypeOfWidget == WidgetItem.WidgetType.Clock)
                {
                    endpoint = $"{_widgetBase}/clocks/post/new";

                    try
                    {
                        await RequestHandler.PostJsonAsync(_httpClient, endpoint, widgetSend);
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else if (widgetSend.TypeOfWidget == WidgetItem.WidgetType.DeviceStatus)
                {
                    endpoint = $"{_widgetBase}/status/post/new";

                    try
                    {
                        await RequestHandler.PostJsonAsync(_httpClient, endpoint, widgetSend);
                        return true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    throw new InvalidOperationException("object is not known widget type");
                }

            }
            else
            {
                throw new InvalidCastException("object is not known item type");
            }
        }

        public async Task<bool> EditItemAsync(DashboardItemBase item)
        {
            if (item is ShortcutItem shortcut)
            {
                string endpoint = $"{_shortcutBase}/put/update";

                try
                {
                    await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, shortcut);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is DirectoryItem directory)
            {
                string endpoint = $"{_directoryBase}/put/update";

                try
                {
                    await RequestHandler.PutAsJsonAsync(_httpClient, endpoint, directory);
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else if (item is WidgetItem widget)
            {
                throw new ArgumentException("cannot edit widgets");
            }
            else
            {
                throw new InvalidCastException("object is not known item type");
            }
        }

        public async Task<bool> ReplaceWholeDashboardAsync(List<DashboardItemBase> newItems)
        {
            List<DashboardItemBase> oldItems = [];
            try
            {
                oldItems = await GetAllItemsAsync();
            } catch { }
            
            bool success = true;

            foreach (var item in oldItems)
            {
                try
                {
                    await DeleteItemAsync(item);
                }
                catch 
                {
                    throw;
                }
            }

            foreach (var item in newItems)
            {
                try
                {
                    await SaveItemAsync(item);
                }
                catch 
                {
                    throw;
                }
            }

            return success;
        }
    }
}
