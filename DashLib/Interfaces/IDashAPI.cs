using DashLib.Dashboard;

namespace DashLib.Interfaces
{
    public interface IDashAPI
    {
        public Task<List<DashboardItemBase>> GetAllItemsAsync();
        public Task<bool> SaveItemAsync(DashboardItemBase item);
        public Task<bool> DeleteItemAsync(DashboardItemBase item);
        public Task<bool> EditItemAsync(DashboardItemBase item);
    }
}