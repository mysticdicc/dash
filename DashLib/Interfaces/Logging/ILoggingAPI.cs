using DashLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Logging
{
    public interface ILoggingAPI
    {
        public Task<List<LogEntry>> GetAllLogEntriesAsync();
    }
}
