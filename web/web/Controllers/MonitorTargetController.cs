using dankweb.API;
using Microsoft.AspNetCore.Mvc;
using DashLib;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using web.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using DashLib.Models.Network;
using DashLib.Interfaces.Monitoring;
using Microsoft.AspNetCore.Authorization;

namespace web.Controllers
{
    [ApiController]
    [Authorize]
    public class MonitorTargetController(DiscoveryService discoveryService, IMonitorTargetRepository subnetRepository) : Controller
    {
        private readonly DiscoveryService _discoveryService = discoveryService;
        private readonly IMonitorTargetRepository _dbRepo = subnetRepository;

        [HttpPost]
        [Route("[controller]/v2/startdiscovery")]
        public async Task<IActionResult> StartSubnetDiscovery(SubnetContainer subnet)
        {
            try
            {
                var discovered = await _discoveryService.ExecuteDiscoveryTasksAsync(subnet);

                if (discovered == null) return Problem("No response from discovery tasks");

                var result = await _dbRepo.SubmitDiscoveryTaskAsync(discovered);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/subnet/post/new")]
        public async Task<IActionResult> AddSubnetContainerAsync(SubnetContainer subnet)
        {
            try
            {
                var result = await _dbRepo.AddSubnetContainerAsync(subnet);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("[controller]/v2/ip/post/new")]
        public async Task<IActionResult> AddIpTargetAsync(IpMonitoringTarget ip)
        {
            try
            {
                var result = await _dbRepo.AddIpTargetAsync(ip);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/dnscont/post/new")]
        public async Task<IActionResult> AddDnsContainerAsync(DnsContainer container)
        {
            try
            {
                var result = await _dbRepo.AddDnsContainerAsync(container);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/dns/post/new")]
        public async Task<IActionResult> AddDnsTargetAsync(DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _dbRepo.AddDnsTargetAsync(dns);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/subnet/update")]
        public async Task<IActionResult> UpdateSubnetContainerAsync(SubnetContainer subnet)
        {
            try
            {
                var result = await _dbRepo.UpdateSubnetContainerAsync(subnet);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/dnscont/update")]
        public async Task<IActionResult> UpdateDnsContainerAsync(DnsContainer container)
        {
            try
            {
                var result = await _dbRepo.UpdateDnsContainerAsync(container);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/ip/update")]
        public async Task<IActionResult> UpdateIpTargetAsync(IpMonitoringTarget ip)
        {
            try
            {
                var result = await _dbRepo.UpdateIpTargetAsync(ip);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/dns/update")]
        public async Task<IActionResult> UpdateDnsTargetAsync(DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _dbRepo.UpdateDnsTargetAsync(dns);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/ip/get/all")]
        public async Task<IActionResult> GetAllIpsAsync()
        {
            try
            {
                var list = await _dbRepo.GetAllIpsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/dns/get/all")]
        public async Task<IActionResult> GetAllDnsTargetsAsync()
        {
            try
            {
                var list = await _dbRepo.GetAllDnsTargetsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/subnet/get/all")]
        public async Task<IActionResult> GetAllSubnetContainersAsync()
        {
            try
            {
                var list = await _dbRepo.GetAllSubnetContainersAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/dnscont/get/all")]
        public async Task<IActionResult> GetAllDnsContainersAsync()
        {
            try
            {
                var list = await _dbRepo.GetAllDnsContainersAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/ip/get/byid")]
        public async Task<IActionResult> GetIpTargetByIdAsync(int id)
        {
            try
            {
                var ip = await _dbRepo.GetIpTargetByIdAsync(id);
                return Ok(ip);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/dnscont/get/byid")]
        public async Task<IActionResult> GetDnsContainerByIdAsync(int id)
        {
            try
            {
                var dns = await _dbRepo.GetDnsContainerByIdAsync(id);
                return Ok(dns);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/subnet/get/byid")]
        public async Task<IActionResult> GetSubnetContainerByIdAsync(int id)
        {
            try
            {
                var subnet = await _dbRepo.GetSubnetContainerByIdAsync(id);
                return Ok(subnet);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/dns/get/byid")]
        public async Task<IActionResult> GetDnsTargetByIdAsync(int id)
        {
            try
            {
                var dns = await _dbRepo.GetDnsTargetByIdAsync(id);
                return Ok(dns);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/subnet/delete")]
        public async Task<IActionResult> DeleteSubnetContainerAsync([FromBody] SubnetContainer subnet)
        {
            try
            {
                var result = await _dbRepo.DeleteSubnetContainerAsync(subnet);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/dnscont/delete")]
        public async Task<IActionResult> DeleteDnsContainerAsync([FromBody] DnsContainer dns)
        {
            try
            {
                var result = await _dbRepo.DeleteDnsContainerAsync(dns);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/ip/delete")]
        public async Task<IActionResult> DeleteIpTargetAsync([FromBody] IpMonitoringTarget ip)
        {
            try
            {
                var result = await _dbRepo.DeleteIpTargetAsync(ip);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/dns/delete")]
        public async Task<IActionResult> DeleteDnsTargetAsync([FromBody] DnsMonitoringTarget dns)
        {
            try
            {
                var result = await _dbRepo.DeleteDnsTargetAsync(dns);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/subnet/delete/byid")]
        public async Task<IActionResult> DeleteSubnetByIdAsync(int id)
        {
            try
            {
                var result = await _dbRepo.DeleteSubnetByIdAsync(id);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/dnscont/delete/byid")]
        public async Task<IActionResult> DeleteDnsContainerByIdAsync(int id)
        {
            try
            {
                var result = await _dbRepo.DeleteDnsContainerByIdAsync(id);

                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return Problem("No changes were made to the database.");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
