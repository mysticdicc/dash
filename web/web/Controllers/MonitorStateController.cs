using dankweb.API;
using DashLib.DTO;
using DashLib.Interfaces.Monitoring;
using DashLib.Models.Monitoring;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class MonitorStateController(IMonitorStateRepository monitoringRepository, MonitorService monitor) : Controller
    {
        private readonly IMonitorStateRepository _dbRepo = monitoringRepository;
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
        public IActionResult RestartServiceAsync()
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
        public async Task<IActionResult> PingDeviceByStringIpAsync(string ip)
        {
            var response = new PingResponseDto()
            {
                IP = ip
            };

            try
            {
                var byteArr = IpMonitoringTarget.ConvertToByte(ip);

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
        public async Task<IActionResult> GetIpMonitoringTargetByStringAddressAsync(string ip)
        {
            try
            {
                var dbIp = await _dbRepo.GetIpMonitoringTargetByStringAddressAsync(ip);
                return Ok(SerializeObject(dbIp));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/deviceandstatus/byadress")]
        public async Task<IActionResult> GetDnsMonitoringTargetByStringAddressAsync(string address)
        {
            try
            {
                var dbDns = await _dbRepo.GetDnsMonitoringTargetByStringAddressAsync(address);
                return Ok(SerializeObject(dbDns));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/deviceandstatus/ips")]
        public async Task<IActionResult> GetMonitoredIpAndStatusAsync()
        {
            try
            {
                var ips = await _dbRepo.GetMonitoredIpAndStatusAsync();
                return Ok(SerializeObject(ips));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/deviceandstatus/dns")]
        public async Task<IActionResult> GetMonitoredDnsAndStatusAsync()
        {
            try
            {
                var dns = await _dbRepo.GetMonitoredDnsAndStatusAsync();
                return Ok(SerializeObject(dns));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/dns")]
        public async Task<IActionResult> GetAllMonitoredDns()
        {
            try
            {
                var dns = await _dbRepo.GetAllMonitoredDnsAsync();
                return Ok(SerializeObject(dns));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/ips")]
        public async Task<IActionResult> GetAllMonitoredIps()
        {
            try
            {
                var ips = await _dbRepo.GetAllMonitoredIpsAsync();
                return Ok(SerializeObject(ips));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/dns/byid")]
        public async Task<IActionResult> GetDnsMonitorStatesByDeviceIdAsync(int id)
        {
            try
            {
                var states = await _dbRepo.GetDnsMonitorStatesByDeviceIdAsync(id);
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/ip/byid")]
        public async Task<IActionResult> GetIpMonitorStatesByDeviceIdAsync(int id)
        {
            try
            {
                var states = await _dbRepo.GetIpMonitorStatesByDeviceIdAsync(id);
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/post/ip")]
        public async Task<IActionResult> AddMonitorStatesFromListIpAsync(List<IpMonitoringTarget> ips)
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

        [HttpPost]
        [Route("[controller]/v2/post/dns")]
        public async Task<IActionResult> AddMonitorStatesFromListDnsAsync(List<DnsMonitoringTarget> dnsList)
        {
            try
            {
                var result = await _dbRepo.AddMonitorStatesFromListDnsAsync(dnsList);

                if (result)
                {
                    return Ok(SerializeObject(dnsList));
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/all/portstates")]
        public async Task<IActionResult> GetAllPortStatesAsync()
        {
            try
            {
                var states = await _dbRepo.GetAllPortStatesAsync();
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/all/pingstates")]
        public async Task<IActionResult> GetAllPingStatesAsync()
        {
            try
            {
                var states = await _dbRepo.GetAllPingStatesAsync();
                return Ok(SerializeObject(states));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
