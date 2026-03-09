using DashLib.Models.Dashboard;

namespace DashLib.Interfaces.Dashboard
{
    public interface IDashAPI
    {
        public Task<List<DashboardItemBase>> GetAllItemsAsync();
        public Task<bool> SaveItemAsync(DashboardItemBase item);
        public Task<bool> DeleteItemAsync(DashboardItemBase item);
        public Task<bool> EditItemAsync(DashboardItemBase item);
        public Task<bool> ReplaceWholeDashboardAsync(List<DashboardItemBase> items);
    }
}