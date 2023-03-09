using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Models;
using BE.Services.InfoDeviceServices;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: infoDevices")]
    public class InfoDevicesController : ControllerBase
    {
        private readonly IInfoDeviceService _infoDeviceService;

        public InfoDevicesController(IInfoDeviceService infoDeviceService)
        {
            _infoDeviceService = infoDeviceService;
        }

        [HttpGet("GetAllDeviceListWithApplication")]
        public async Task<IActionResult> GetAllDeviceList()
        {
            var response = await _infoDeviceService.GetAllAsync();
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("CreateDeviceWithListApplication")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: infoDevices add: 1")]
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

        [HttpPut("EditDeviceWithListApplicationByDeviceId/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: infoDevices update: 1")]
        public async Task<IActionResult> EditInfoDevice(int id , CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.EditInfoDeviceWithDeviceId(id, createInfoDeviceDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserName/{name}")]
        public async Task<IActionResult> GetWithName(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithName(name);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithOperatingSystem/{operatingSystem}")]
        public async Task<IActionResult> GetWithOperatingSystem(int operatingSystem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithOperatingSystem(operatingSystem);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserCode/{userCode}")]
        public async Task<IActionResult> GetWithUserCode(string userCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithUserCode(userCode);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("FindWithUserId/{userId}")]
        public async Task<IActionResult> GetWithUserId(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.FindWithUserId(userId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("EditDeviceWithListApplicationByUserId/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: infoDevices update: 1")]
        public async Task<IActionResult> EditInfoDeviceWithUserId(int id, CreateInfoDeviceDto createInfoDeviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _infoDeviceService.EditInfoDeviceWithUserId(id, createInfoDeviceDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
