using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashLib.Models.Settings;

namespace DashLib.Interfaces
{
    public interface ISettingsAPI
    {
        public Task<AllSettings> GetCurrentSettingsAsync();
        public Task<bool> CreateNewSettingsAsync(AllSettings newSettings);
    }
}
