using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.LeaveOffServices;
using BE.Services.PaginationServices;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using static BE.Data.Enum.LeaveOff.Status;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/leaveOff")]
    public class LeaveOffController : Controller
    {
        private readonly ILeaveOffServices _leaveOffServices;
        private readonly IPaginationServices<LeaveOff> _paginationServices;

        public LeaveOffController(ILeaveOffServices leaveOffServices, IPaginationServices<LeaveOff> paginationServices)
        {
            _leaveOffServices = leaveOffServices;
            _paginationServices = paginationServices;
        }

        //GET: get all leave off
        [HttpGet("getAllLeaveOff")]
        public async Task<IActionResult> getAllLeaveOff(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllAsync();
            if(response._success)
            {
                var pageSize = (int)pageSizeEnum;
                var resultPage = await _paginationServices.paginationListTableAsync(response._Data, pageIndex, pageSize);
                if(resultPage._success)
                {
                    return Ok(resultPage);
                }
                return BadRequest(resultPage);
            }
            return BadRequest(response);
        }

        //GET: get all leave off by status
        [HttpGet("getAllLeaveOffByStatus")]
        public async Task<IActionResult> getAllLeaveOffByStatus(StatusLO statusLO, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByStatus(statusLO);
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

        //GET: get all leave off of register by status
        [HttpGet("getAllLeaveOffOfidUserByStatus/{idLeaveOffUser}")]
        public async Task<IActionResult> getAllLeaveOffOfIdUserByStatus(int idLeaveOffUser, StatusLO statusLO, int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _leaveOffServices.GetAllLeaveOffOfIdUserByStatus(idLeaveOffUser,statusLO);
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

        //GET: get leave off by idLeaveOff
        [HttpGet("getLeaveOffById/{idLeaveOff}")]
        public async Task<IActionResult> getLeaveOffById(int idLeaveOff)
        {
            var response = await _leaveOffServices.GetLeaveOffByIdAsync(idLeaveOff);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //POST: add a new leave off
        [HttpPost("addNewLeaveOff")]
        public async Task<IActionResult> addNewLeaveOff(AddNewLeaveOffDto addNewLeaveOffDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }    
         
            var response = await _leaveOffServices.AddNewLeaveOffAsync(addNewLeaveOffDto);
            if(response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //PUT: edit form leave off of registering
        [HttpPut("editRegisterLeaveOff/{id}")]
        public async Task<IActionResult> editRegisterLeaveOff(int id, EditRegisterLeaveOffDtos editRegisterLeaveOffDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leaveOff = await _leaveOffServices.EditRegisterLeaveOffAsync(id, editRegisterLeaveOffDtos);
            if(leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //PUT: accept form leave off 
        [HttpPut("accceptLeaveOff/{idLeaveOff}")]
        public async Task<IActionResult> acceptRegisterLeaveOff(int idLeaveOff, AccepterLeaveOffDto accepterLeaveOffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var leaveOff = await _leaveOffServices.AccepterLeaveOffAsync(idLeaveOff, accepterLeaveOffDto.idAcceptUser);
            if (leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //PUT: not accept form leave off 
        [HttpPut("notAcceptLeaveOff/{idLeaveOff}")]
        public async Task<IActionResult> notAcceptRegisterLeaveOff(int idLeaveOff, NotAcceptLeaveOffDto notAcceptLeaveOffDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leaveOff = await _leaveOffServices.NotAccepterLeaveOffAsync(idLeaveOff, notAcceptLeaveOffDto);
            if (leaveOff._success)
            {
                return Ok(leaveOff);
            }

            return BadRequest(leaveOff);
        }

        //DELETE: delete leave off by idLeaveOff
        [HttpDelete("deleteLeaveOffById/{idLeaveOff}")]
        public async Task<IActionResult> deleteLeaveOffById(int idLeaveOff)
        {
            var response = await _leaveOffServices.DeleteLeaveOffByIdAsync(idLeaveOff);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("GetByNameStatusDate")]
        public async Task<IActionResult> getLeaveOffByNameStatusDate(FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByNameStatusDate(findByNameStatusDateDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("GetAllLeaveOffByYear")]
        public async Task<IActionResult> getAllLeaveOffByYear(FindByNameStatusDateDtos findByNameStatusDateDtos)
        {
            var response = await _leaveOffServices.GetAllLeaveOffByYear(findByNameStatusDateDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
