using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashLib.Interfaces
{
    public interface IMailAPI
    {
        Task<bool> SendTestEmailAsync();
    }
}
