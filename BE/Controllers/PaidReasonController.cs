using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.Customers;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "permission_group: True module: paidResons")]
    public class PaidReasonController : ControllerBase
    {
        private readonly IPaidReasonsService _paidReasonService;
        private readonly IPaginationServices<PaidReasons> _paginationService;

        public PaidReasonController(IPaidReasonsService paidReasonService, IPaginationServices<PaidReasons> paginationServices)
        {
            _paidReasonService = paidReasonService;
            _paginationService = paginationServices;
        }

        [HttpGet("getAllPaidReason")]
        public async Task<IActionResult> GetAllPaid(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _paidReasonService.GetAllAsync();
            if (response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if (resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _paidReasonService.GetById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
