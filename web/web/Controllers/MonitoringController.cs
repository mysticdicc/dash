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

namespace web.Controllers
{
    [ApiController]
    public class MonitoringController(IDbContextFactory<DashDbContext> dbContext, MonitorService monitor) : Controller
    {
        private readonly IDbContextFactory<DashDbContext> _DbFactory = dbContext;
        private readonly MonitorService _monitorService = monitor;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        [HttpGet]
        [Route("[controller]/v2/service/restart")]
        public IActionResult RestartService()
        {
            _monitorService.Restart();
            return Ok("Restarted");
        }

        [HttpGet]
        [Route("[controller]/v2/get/devicesandstatus")]
        public string GetDevicesAndMonitorStates()
        {
            using var context = _DbFactory.CreateDbContext();
            var result = context.IPs
                .Where(x => (x.IsMonitoredTCP || x.IsMonitoredICMP) && x.MonitorStateList != null)
                .Include(x => x.MonitorStateList!)
                    .ThenInclude(x => x.PortState)
                .Include(x => x.MonitorStateList!)
                    .ThenInclude(x => x.PingState)
                .ToList();
            return JsonSerializer.Serialize(result, JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/get/all")]
        public string GetAllMonitoredDevices()
        {
            using var context = _DbFactory.CreateDbContext();
            var result = context.IPs.Where(x => x.IsMonitoredICMP || x.IsMonitoredTCP).ToList();
            return JsonSerializer.Serialize(result, JsonOptions);
        }

        [HttpGet]
        [Route("[controller]/v2/get/byid")]
        public string GetByDeviceID(int ID)
        {
            using var context = _DbFactory.CreateDbContext();
            var result = context.MonitorStates
                .Where(x => x.IP_ID == ID)
                .Include(x => x.PortState)
                .Include(x => x.PingState)
                .ToList();
            return JsonSerializer.Serialize(result, JsonOptions);
        }

        [HttpPost]
        [Route("[controller]/v2/new/polls")]
        public async Task<Results<BadRequest<string>, Ok>> NewDevicePoll(List<IP> ips)
        {
            using var context = _DbFactory.CreateDbContext();
            var monitorStates = ips.SelectMany(x => x.MonitorStateList!);

            try
            {
                context.MonitorStates.AddRange(monitorStates);
                await context.SaveChangesAsync();
                return TypedResults.Ok();
            } 
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/allpolls")]
        public string GetAllPolls()
        {
            using var context = _DbFactory.CreateDbContext();
            var allStates = context.MonitorStates
                .Include(x => x.PingState)
                .Include(x => x.PortState)
                .ToList();
            return JsonSerializer.Serialize(allStates, JsonOptions);
        }
    }
}
