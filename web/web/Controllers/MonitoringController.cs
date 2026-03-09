using dankweb.API;
using DashLib.DTO;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using web.Services;

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
            try
            {
                _monitorService.Restart();
                return Ok("Restarted");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/pingstatus")]
        public async Task<IActionResult> GetPingStatus(string ip)
        {
            var response = new PingResponseDto()
            {
                IP = ip
            };

            try
            {
                var byteArr = IP.ConvertToByte(ip);

                if (byteArr == null) return BadRequest("Invalid IP");

                using Ping ping = new();
                var addr = new IPAddress(byteArr);

                var status = await ping.SendPingAsync(addr, 5000);

                response.IcmpResponse = status.Status == IPStatus.Success;

                if (!response.IcmpResponse)
                {
                    response.Exception = $"Ping failed with status: {status.Status}";
                }

                return Ok(SerializeObject(response));
            }
            catch(Exception ex)
            {
                response.IcmpResponse = false;
                response.Exception = ex.Message;
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/deviceandstatus/byip")]
        public async Task<IActionResult> GetDeviceAndMonitorStatesByIp(string ip)
        {
            try
            {
                var dbIp = await _dbRepo.GetDeviceAndMonitorStatesByStringIpAsync(ip);
                return Ok(SerializeObject(dbIp));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
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
