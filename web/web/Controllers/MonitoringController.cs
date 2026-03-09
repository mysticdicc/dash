using dankweb.API;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using web.Services;
using DashLib.Monitoring;
using DashLib.Network;
using System.Text.Json;
using System.Text.Json.Serialization;
using DashLib.Settings;
using DashLib.Interfaces.Monitoring;

namespace web.Controllers
{
    [ApiController]
    public class MonitoringController(IMonitoringRepository monitoringRepository, MonitorService monitor) : Controller
    {
        private readonly IMonitoringRepository _dbRepo = monitoringRepository;
        private readonly MonitorService _monitorService = monitor;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        private static string SerializeObject(object obj)
        {
            return JsonSerializer.Serialize(obj, JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/service/restart")]
        public IActionResult RestartService()
        {
            _monitorService.Restart();
            return Ok("Restarted");
        }

        [HttpGet]
        [Route("[controller]/v2/get/devicesandstatus")]
        public async Task<IActionResult> GetDevicesAndMonitorStates()
        {
            try
            {
                var ips = await _dbRepo.GetMonitoredDevicesAndStatusAsync();
                return Ok(SerializeObject(ips));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/all")]
        public async Task<IActionResult> GetAllMonitoredDevices()
        {
            try
            {
                var ips = await _dbRepo.GetAllMonitoredDevicesAsync();
                return Ok(SerializeObject(ips));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/byid")]
        public async Task<IActionResult> GetByDeviceID(int ID)
        {
            try
            {
                var states = await _dbRepo.GetMonitorStatesByDeviceIdAsync(ID);
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/new/polls")]
        public async Task<IActionResult> NewDevicePoll(List<IP> ips)
        {
            try
            {
                var result = await _dbRepo.AddMonitorStatesFromListIpAsync(ips);

                if (result)
                {
                    return Ok(SerializeObject(ips));
                }
                else
                {
                    return Problem("No changed were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/allpolls")]
        public async Task<IActionResult> GetAllPolls()
        {
            try
            {
                var states = await _dbRepo.GetAllMonitorStatesAsync();
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
