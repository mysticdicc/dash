using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using danklibrary.Settings;

namespace danklibrary.Interfaces
{
    public interface ISettingsAPI
    {
        public Task<AllSettings> GetCurrentSettingsAsync();
        public Task<bool> CreateNewSettingsAsync(AllSettings newSettings);
    }
}
