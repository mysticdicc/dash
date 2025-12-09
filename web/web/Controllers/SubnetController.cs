using dankweb.API;
using Microsoft.AspNetCore.Mvc;
using danklibrary;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using web.Services;
using danklibrary.Network;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace web.Controllers
{
    [ApiController]
    public class SubnetsController(IDbContextFactory<danknetContext> dbContext, DiscoveryService discoveryService) : Controller
    {
        private readonly IDbContextFactory<danknetContext> _DbFactory = dbContext;
        private readonly DiscoveryService _discoveryService = discoveryService;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = null,
            WriteIndented = true
        };

        [HttpPost]
        [Route("[controller]/v2/startdiscovery")]
        public async Task<Results<BadRequest<string>, Ok<Subnet>>> StartSubnetDiscovery(Subnet subnet)
        {

            using var context = _DbFactory.CreateDbContext();
            var _subnet = await _discoveryService.StartDiscovery(subnet);

            if (null != _subnet)
            {
                var dbSubnet = context.Subnets.Include(x => x.List).FirstOrDefault(x => x.ID == _subnet.ID);

                if (null != dbSubnet)
                {
                    try
                    {
                        dbSubnet = _subnet;

                        foreach (var ip in _subnet.List)
                        {
                            var dbIp = context.IPs.Find(ip.ID);

                            if (null != dbIp)
                            {
                                dbIp = ip;
                            }
                        }

                        await context.SaveChangesAsync();
                        return TypedResults.Ok(_subnet);
                    }
                    catch (Exception ex)
                    {
                        return TypedResults.BadRequest(ex.Message);
                    }

                }
                else
                {
                    return TypedResults.BadRequest("Unable to find db object");
                }
            }
            else
            {
                return TypedResults.BadRequest("Null object return from discovery service");
            }
        }

        [HttpPost]
        [Route("[controller]/v2/new/byobject")]
        public async Task<Results<BadRequest<string>, Created<Subnet>>> AddSubnetByObject(Subnet subnet)
        {
            using var context = _DbFactory.CreateDbContext();

            try
            {
                context.Subnets.Add(subnet);
                await context.SaveChangesAsync();

                return TypedResults.Created(subnet.ID.ToString(), subnet);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[controller]/v2/update/byobject")]
        public async Task<Results<BadRequest<string>, Ok<Subnet>>> EditSubnetByObject(Subnet subnet)
        {
            using var context = _DbFactory.CreateDbContext();
            var item = context.Subnets.Find(subnet.ID);

            if (null == item)
                return TypedResults.BadRequest("Unable to find matching ID");

            item.List = subnet.List;
            item.Address = subnet.Address;
            item.StartAddress = subnet.StartAddress;
            item.EndAddress = subnet.EndAddress;
            item.SubnetMask = subnet.SubnetMask;

            try
            {
                await context.SaveChangesAsync();
                return TypedResults.Ok(subnet);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/all")]
        public string GetAllSubnets()
        {
            using var context = _DbFactory.CreateDbContext();

            List<Subnet> tempSubnet = context.Subnets.ToList();

            foreach (Subnet subnet in tempSubnet)
            {
                subnet.List = context.IPs.Where(x => x.SubnetID == subnet.ID).ToList();
            }

            return JsonSerializer.Serialize(tempSubnet, JsonOptions);
        }

        [HttpDelete]
        [Route("[controller]/v2/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<int>>> DeleteSubnetByObject(Subnet subnet)
        {
            using var context = _DbFactory.CreateDbContext();
            Subnet? deleteItem = context.Subnets.Find(subnet.ID);

            if (null != deleteItem)
            {
                try
                {
                    context.Subnets.Remove(deleteItem);
                    await context.SaveChangesAsync();
                    return TypedResults.Ok(deleteItem.ID);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest("unable to find ID in db");
            }
        }

        [HttpPost]
        [Route("[controller]/v2/ip/post/new")]
        public async Task<Results<BadRequest<string>, Created<IP>>> AddIP(IP ip)
        {
            using var context = _DbFactory.CreateDbContext();

            //validate
            bool validObject = true;

            if (validObject)
            {
                try
                {
                    if (null != ip.PortsMonitored)
                    {
                        ip.IsMonitoredTCP = true;
                    }

                    context.IPs.Add(ip);
                    await context.SaveChangesAsync();

                    return TypedResults.Created(ip.ID.ToString(), ip);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest("Object invalid.");
            }
        }

        [HttpGet]
        [Route("[controller]/v2/ip/get/all")]
        public string GetAllIP()
        {
            using var context = _DbFactory.CreateDbContext();
            return JsonSerializer.Serialize(context.IPs.ToList(), JsonOptions);
        }

        [HttpPut]
        [Route("[controller]/v2/ip/put/update")]
        public async Task<Results<BadRequest<string>, Ok<IP>>> UpdateIP(IP ip)
        {
            using var context = _DbFactory.CreateDbContext();

            var updateItem = context.IPs.Find(ip.ID);

            if (updateItem != null)
            {
                updateItem.Hostname = ip.Hostname;
                updateItem.IsMonitoredICMP = ip.IsMonitoredICMP;

                if (null != ip.PortsMonitored)
                {
                    updateItem.IsMonitoredTCP = true;
                    updateItem.PortsMonitored = ip.PortsMonitored;
                }

                context.IPs.Update(updateItem);

                await context.SaveChangesAsync();
                return TypedResults.Ok(updateItem);
            }
            else
            {
                return TypedResults.BadRequest($"Unable to find item with {ip.ID}");
            }
        }

        [HttpPost]
        [Route("[controller]/v2/subnet/post/discoveryupdate")]
        public async Task<Results<Ok<List<int>>, Ok<Subnet>>> UpdateSubnet(Subnet subnet)
        {
            using var context = _DbFactory.CreateDbContext();
            List<int> badId = [];

            foreach (var ip in subnet.List)
            {
                var item = context.IPs.Find(ip.ID);
                if (null != item)
                {
                    item.Hostname = ip.Hostname;
                    item.IsMonitoredICMP = ip.IsMonitoredICMP;
                    context.IPs.Update(item);
                }
                else
                {
                    badId.Add(ip.ID);
                }
            }

            await context.SaveChangesAsync();

            if (badId.Count > 0)
            {
                return TypedResults.Ok<List<int>>(badId);
            }
            else
            {
                return TypedResults.Ok<Subnet>(subnet);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/get/byid")]
        public string GetById(int ID)
        {
            using var context = _DbFactory.CreateDbContext();
            var item = context.Subnets.Find(ID);
            return JsonSerializer.Serialize(item, JsonOptions);
        }

        [HttpDelete]
        [Route("[controller]/v2/ip/delete/byobject")]
        public async Task<Results<BadRequest<string>, Ok<int>>> DeleteIPByObject(IP ip)
        {
            using var context = _DbFactory.CreateDbContext();
            IP? deleteItem = context.IPs.Include(x => x.Subnet).Where(x => x.ID == ip.ID).FirstOrDefault();

            if (null != deleteItem)
            {
                try
                {
                    if (null != deleteItem.Subnet)
                    {
                        var subnet = context.Subnets.Find(deleteItem.Subnet.ID);

                        if (null != subnet)
                        {
                            subnet.List.Remove(ip);
                        }
                    }

                    context.IPs.Remove(deleteItem);
                    await context.SaveChangesAsync();
                    return TypedResults.Ok(deleteItem.ID);
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest(ex.Message);
                }
            }
            else
            {
                return TypedResults.BadRequest("unable to find ID in db");
            }
        }

    }
}
