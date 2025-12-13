using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DashLib.Network;

namespace DashLib.Interfaces
{
    public interface IMailAPI
    {
        Task<bool> SendTestEmailAsync();
        Task<bool> SendAlertEmailAsync(List<IP> ipList);
    }
}
