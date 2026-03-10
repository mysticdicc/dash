using DashLib.Interfaces.Logging;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [ApiController]
    public class LogsController(ILoggingRepository loggingRepository) : Controller
    {
        private readonly ILoggingRepository _loggingRepository = loggingRepository;

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
