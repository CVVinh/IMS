using BE.Data.Contexts;
using BE.Data.Dtos.DeviceDto;
using BE.Data.Dtos.DeviceDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: devices")]
    public class DevicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public DevicesController(AppDbContext context)
        {
            _context = context;
           
        }

        [HttpGet("getListDevices")]
        public ActionResult getListDevices()
        {
            try
            {
                var dsDevices = _context.Devices.ToList();
                return Ok(dsDevices);

            }
            catch (Exception ew)
            {
                return BadRequest(ew);
            }
        }
        [HttpPost("addDevices")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devices add: 1")]
        public async Task<IActionResult> addDevices(AddDeviceDto req)
        {
            try
            {
                var addDevices = await _context.Devices.SingleOrDefaultAsync(t => t.DeviceName!.ToLower() == req.DeviceName.ToLower());
                if (addDevices == null)
                {
                    var dev = new Devices();
                    dev.DeviceName = req.DeviceName;
                    dev.Info = req.Info;
                    dev.UserCreated = req.UserCreated;
                    dev.Note = req.Note;
                    dev.DateCreated = DateTime.UtcNow;
                    _context.Devices.Add(dev);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return Ok("Da Ton Tai");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
           
        }

        [HttpPut]
        [Route("updateDevices")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devices update: 1")]
        public async Task<ActionResult> updateDevices(Devices requests)
        {
            var Devices = await _context.Devices.FindAsync(requests.IdDevice);
            if (Devices == null)
            {
                return NotFound("Khong tim thay du lieu");
            }
            Devices.DeviceName = requests.DeviceName;
            Devices.IdDevice = requests.IdDevice; 
            Devices.Note = requests.Note;
            Devices.Info = requests.Info;
            Devices.UserUpdated = requests.UserUpdated;
            Devices.DateUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("DeleteDevices/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: devices delete: 1")]
        public async Task<ActionResult> DeleteDevices(int id, IdUserChangeDeviceDto request)
        {
            try
            {
                var devices =  await _context.Devices.SingleOrDefaultAsync(p => p.IdDevice == id);
                if (devices == null)
                {
                    return NotFound("Khong tim thay doi tuong");
                }
                else
                {
                    devices.IsDeleted = 1;
                    devices.UserUpdated = request.UserId;
                    devices.DateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return Ok();
                    
                }

            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
            
        }
        [HttpPost("getPaginate")]
        public async Task<ActionResult> GetAllPaging(PaginateDevices pd)
        {
            try
            {
                if (pd.recordNum == 0)
                {
                    pd.recordNum = 10;
                }
                if (pd.page == 0)
                {
                    pd.page = 1;
                }
                var devices = await _context.Devices
                    .Skip((pd.page - 1) * pd.recordNum)
                    .Take(pd.recordNum).ToListAsync();
                if (devices == null)
                    return NotFound();
                var pageCount = Math.Ceiling(devices.Count() / Convert.ToDecimal(pd.recordNum));
                var Device = devices
                  //  .Skip((pd.page - 1) * pd.recordNum)
                    .Take(pd.recordNum).ToList();
                var response = new Devicesreponse
                {
                    Devices = Device,
                    CurrentPage = pd.page,
                    Pages = (int)pageCount
                };

                return Ok(response);

            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        } 

    }
 }


