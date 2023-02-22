using BE.Data.Dtos.InfoDeviceDtos;
using BE.Services.InfoDeviceServices;
using BE.Services.LeaveOffServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoDevicesController : ControllerBase
    {
        private readonly IInfoDeviceService _infoDeviceService;

        public InfoDevicesController(IInfoDeviceService infoDeviceService)
        {
            _infoDeviceService = infoDeviceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDeviceList()
        {
            var response = await _infoDeviceService.GetAllAsync();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost]
        public async Task<IActionResult> CreateInfoDevice(CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _infoDeviceService.CreateInfoDevice(createInfoDeviceDto);

            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        public async Task<IActionResult> EditInfoDevice(int id , CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.EditInfoDevice(id, createInfoDeviceDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
