using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using danklibrary.Settings;

namespace danklibrary.DankAPI
{
    public interface ISettingsAPI
    {
        public Task<AllSettings> GetCurrentSettingsAsync();
        public Task<bool> CreateNewSettingsAsync(AllSettings newSettings);
    }
}
