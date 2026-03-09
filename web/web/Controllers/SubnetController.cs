using dankweb.API;
using Microsoft.AspNetCore.Mvc;
using DashLib;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using web.Services;
using DashLib.Network;
using System.Text.Json;
using System.Text.Json.Serialization;
using DashLib.Interfaces.Network;

namespace web.Controllers
{
    [ApiController]
    public class SubnetsController(DiscoveryService discoveryService, ISubnetRepository subnetRepository) : Controller
    {
        private readonly DiscoveryService _discoveryService = discoveryService;
        private readonly ISubnetRepository _dbRepo = subnetRepository;

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

        [HttpPost]
        [Route("[controller]/v2/startdiscovery")]
        public async Task<IActionResult> StartSubnetDiscovery(Subnet subnet)
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
        [Route("[controller]/v2/new/byobject")]
        public async Task<IActionResult> AddSubnetByObject(Subnet subnet)
        {
            try
            {
                var result = await _dbRepo.AddSubnetAsync(subnet);

                if (result)
                {
                    return Ok(SerializeObject(subnet)); 
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
        [Route("[controller]/v2/update/byobject")]
        public async Task<IActionResult> EditSubnetByObject(Subnet subnet)
        {
            try
            {
                var result = await _dbRepo.UpdateSubnetAsync(subnet);

                if (result)
                {
                    return Ok(SerializeObject(subnet));
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
        [Route("[controller]/v2/ips/get/all")]
        public async Task<IActionResult> GetAllIps()
        {
            try
            {
                var list = await _dbRepo.GetAllIpsAsync();
                return Ok(SerializeObject(list));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/ips/get/byid")]
        public async Task<IActionResult> GetIpById(int ID)
        {
            try
            {
                var ip = await _dbRepo.GetIpByIdAsync(ID);
                return Ok(SerializeObject(ip));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); 
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/all")]
        public async Task<IActionResult> GetAllSubnets()
        {
            try
            {
                var list = await _dbRepo.GetAllSubnetsWithIpsAsync();
                return Ok(SerializeObject(list));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/delete/byobject")]
        public async Task<IActionResult> DeleteSubnetByObject(Subnet subnet)
        {
            try
            {
                var result = await _dbRepo.DeleteSubnetAsync(subnet);

                if (result)
                {
                    return Ok(SerializeObject(subnet));
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
        public async Task<IActionResult> AddIP(IP ip)
        {
            try
            {
                var result = await _dbRepo.AddNewIpAsync(ip);

                if (result)
                {
                    return Ok(SerializeObject(ip));
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
        public async Task<IActionResult> GetAllIP()
        {
            try
            {
                var list = await _dbRepo.GetAllIpsAsync();
                return Ok(SerializeObject(list));
            }
            catch (Exception ex) 
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/ip/put/update")]
        public async Task<IActionResult> UpdateIP(IP ip)
        {
            try
            {
                var result = await _dbRepo.UpdateIpAsync(ip);

                if (result)
                {
                    return Ok(SerializeObject(ip));
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
        [Route("[controller]/v2/get/byid")]
        public async Task<IActionResult> GetById(int ID)
        {
            try
            {
                var ip = await _dbRepo.GetIpByIdAsync(ID);
                return Ok(SerializeObject(ip));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); 
            }
        }

        [HttpDelete]
        [Route("[controller]/v2/ip/delete/byobject")]
        public async Task<IActionResult> DeleteIPByObject(IP ip)
        {
            try
            {
                var result = await _dbRepo.DeleteIpAsync(ip);

                if (result)
                {
                    return Ok(SerializeObject(ip));
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
