﻿using BE.Data.Dtos.PaidDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaidController : Controller
    {
        private readonly IPaidServices _paidServices;
        private readonly IPaginationServices<Paid> _paginationServices;
        private readonly IWebHostEnvironment _host;

        public PaidController(IPaidServices paidServices, IPaginationServices<Paid> paginationServices, IWebHostEnvironment host)
        {
            _paidServices = paidServices;
            _paginationServices = paginationServices;
            _host = host;
        }

        [HttpGet]
        [Authorize(Roles = "permission_group: True module: paid")]
        public async Task<IActionResult> GetAllPaid(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidServices.GetAllAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }
        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetPaidWithUserId(int id)
        {
            var response = await _paidServices.GetPaidWithUserId(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaid([FromForm] CreatePaidDtos createPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var response = await _paidServices.CreatePaid(createPaidDtos,_host.WebRootPath, pathServer);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPaid(int id,[FromForm] CreatePaidDtos createPaidDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var pathServer = $"{Request.Scheme}://{Request.Host}";
            var paid = await _paidServices.EditPaid(id, createPaidDtos, _host.WebRootPath, pathServer);
            if (paid._success)
            {
                return Ok(paid);
            }

            return BadRequest(paid);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaid(int id)
        {
            var response = await _paidServices.DeletePaid(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetPaidWithId(int id)
        {
            var response = await _paidServices.GetPaidWithId(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
