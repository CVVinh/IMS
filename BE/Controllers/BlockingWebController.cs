using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.BlockingWebDto;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.NotificationDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Hubs;
using BE.Services.LeaveOffServices;
using BE.Services.PaginationServices;
using BE.shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: blockingWebs")]
    public class BlockingWebController : ControllerBase
    {
        private readonly IBlockingWebService _blockingWebService;
        private readonly IPaginationServices<BlockingWeb> _paginationServices;
        public BlockingWebController(IBlockingWebService blockingWebService, IPaginationServices<BlockingWeb> paginationServices)
        {
            _blockingWebService = blockingWebService;
            _paginationServices = paginationServices;
        }

        [HttpPost("AddBlockingWeb")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: blockingWebs add: 1")]
        public async Task<IActionResult> AddBlockingWeb(AddBlockingWebDto addBlocking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _blockingWebService.AddBlockingWebAsync(addBlocking);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetAllBlockWeb")]
        public async Task<IActionResult> GetAllBlockWeb(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _blockingWebService.GetAllBlockWebAsync();
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
    }
}
