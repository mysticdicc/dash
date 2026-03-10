using DashLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashLib.Interfaces.Logging
{
    public interface ILoggingRepository
    {
        public Task<IReadOnlyList<LogEntry>> GetAllLogsAsync();
        public Task<bool> DeleteAllLogsAsync();
    }
}
