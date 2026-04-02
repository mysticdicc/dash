
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using DashLib.Interfaces.Dashboard;
using DashLib.Models.Dashboard;

namespace web.Controllers
{
    [ApiController]
    public class DashboardController(IDashboardRepository dashboardRepository) : Controller
    {
        private readonly IDashboardRepository _dbRepo = dashboardRepository;

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
        [Route("[controller]/v2/widgets/status/get/all")]
        public async Task<IActionResult> GetAllStatusWidgets()
        {
            try
            {
                var widgets = await _dbRepo.GetStatusWidgetsAsync();
                return Ok(SerializeObject(widgets));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/widgets/clocks/get/all")]
        public async Task<IActionResult> GetAllClockWidgets()
        {
            try
            {
                var widgets = await _dbRepo.GetClockWidgetsAsync();
                return Ok(SerializeObject(widgets));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/shortcuts/get/all")]
        public async Task<IActionResult> GetAllShortcuts()
        {
            try
            {
                var shortcuts = await _dbRepo.GetAllShortcutsAsync();
                return Ok(SerializeObject(shortcuts));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); 
            }
        }

        [HttpGet]
        [Route("[controller]/v2/shortcuts/get/noparent")]
        public async Task<IActionResult> GetShortcutsWithoutParent()
        {
            try
            {
                var shortcuts = await _dbRepo.GetShortcutsWithNoParentAsync();
                return Ok(SerializeObject(shortcuts));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("[controller]/v2/directories/get/all")]
        public async Task<IActionResult> GetAllDirectories()
        {
            try
            {
                var directories = await _dbRepo.GetAllDirectoriesAsync();
                return Ok(SerializeObject(directories));
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/shortcuts/post/new")]
        public async Task<IActionResult> NewShortcut(ShortcutItem item)
        {
            try
            {
                var result = await _dbRepo.AddShortcutAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
                }
                else
                {
                    return Problem("No changed were made to the database.");
                }
            }
            catch(Exception ex)
            { 
                return Problem(ex.Message); 
            }
        }

        [HttpPost]
        [Route("[controller]/v2/widgets/clocks/post/new")]
        public async Task<IActionResult> NewClockWidget(ClockWidget item)
        {
            try
            {
                var result = await _dbRepo.AddClockWidgetAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
                }
                else
                {
                    return Problem("No changed were made to the database.");
                }
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v2/widgets/status/post/new")]
        public async Task<IActionResult> NewDeviceStatuskWidget(DeviceStatusWidget item)
        {
            try
            {
                var result = await _dbRepo.AddStatusWidgetAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
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
        [Route("[controller]/v2/directories/post/new")]
        public async Task<IActionResult> NewDirectory(DirectoryItem item)
        {
            try
            {
                var result = await _dbRepo.AddDirectoryAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
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

        [HttpPut]
        [Route("[controller]/v2/shortcuts/put/update")]
        public async Task<IActionResult> UpdateShortcut(ShortcutItem item)
        {
            try
            {
                var result = await _dbRepo.UpdateShortcutAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
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

        [HttpPut]
        [Route("[controller]/v2/directories/put/update")]
        public async Task<IActionResult> UpdateDirectory(DirectoryItem item)
        {
            try
            {
                var result = await _dbRepo.UpdateDirectoryAsync(item);

                if (result)
                {
                    return Created(item.Id.ToString(), SerializeObject(item));
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

        [HttpDelete]
        [Route("[controller]/v2/widgets/clocks/delete/byobject")]
        public async Task<IActionResult> DeleteClockWidget(ClockWidget item)
        {
            try
            {
                var result = await _dbRepo.DeleteClockWidgetAsync(item);

                if (result)
                {
                    return Ok(SerializeObject(item));
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

        [HttpDelete]
        [Route("[controller]/v2/widgets/status/delete/byobject")]
        public async Task<IActionResult> DeleteStatusWidget(DeviceStatusWidget item)
        {
            try
            {
                var result = await _dbRepo.DeleteStatusWidgetAsync(item);

                if (result)
                {
                    return Ok(SerializeObject(item));
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

        [HttpDelete]
        [Route("[controller]/v2/shortcuts/delete/byobject")]
        public async Task<IActionResult> DeleteShortcut(ShortcutItem item)
        {
            try
            {
                var result = await _dbRepo.DeleteShortcutAsync(item);

                if (result)
                {
                    return Ok(SerializeObject(item));
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

        [HttpDelete]
        [Route("[controller]/v2/directories/delete/byobject")]
        public async Task<IActionResult> DeleteDirectory(DirectoryItem item)
        {
            try
            {
                var result = await _dbRepo.DeleteDirectoryAsync(item);

                if (result)
                {
                    return Ok(SerializeObject(item));
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
    }
}
