using DashLib.Interfaces.Logging;
using DashLib.Models;
using Microsoft.AspNetCore.Mvc;
using web.Services;

namespace web.Controllers
{
    [ApiController]
    public class LogsController(ILoggingRepository loggingRepository, LoggingService loggingService) : Controller
    {
        private readonly ILoggingRepository _loggingRepository = loggingRepository;
        private readonly LoggingService _loggingService = loggingService;
        private static LogEntry.LogSource _logSource = LogEntry.LogSource.ApiController;

        [HttpGet]
        [Route("[controller]/get/all")]
        public async Task<IActionResult> GetAllLogs()
        {


            try
            {
                var logs = await _loggingRepository.GetAllLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
