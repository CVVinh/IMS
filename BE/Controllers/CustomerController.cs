using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.Customers;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using BE.shared.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPaginationServices<Customer> _paginationService;

        public CustomerController(ICustomerService customerService, IPaginationServices<Customer> paginationServices)
        {
            _customerService = customerService;
            _paginationService = paginationServices;
        }

        [HttpGet("getAllCustomer")]
        public async Task<IActionResult> GetAllPaid(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _customerService.GetAllAsync();
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
            var response = await _customerService.GetById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }



    }
}
