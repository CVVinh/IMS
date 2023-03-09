using BE.Data.Dtos.ActionModuleDtos;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.ActionModuleServices;
using BE.Services.GroupServices;
using BE.Services.PaginationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: actionModules")]
    public class ActionModuleController : ControllerBase
    {
        private readonly IActionModuleServices _actionModuleServices;
        private readonly IPaginationServices<Action_Module> _paginationService;

        public ActionModuleController(IActionModuleServices actionModuleServices, IPaginationServices<Action_Module> paginationService)
        {
            _actionModuleServices = actionModuleServices;
            _paginationService = paginationService;
        }

        [HttpGet("getAllActionModule")]
        public async Task<IActionResult> getAllActionModuleAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _actionModuleServices.GetAllActionModuleAsync();
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

        [HttpGet("getActionModuleById/{idAction}")]
        public async Task<IActionResult> GetActionModuleById([FromRoute] int idAction)
        {
            var response = await _actionModuleServices.GetActionModuleById(idAction);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createActionModule")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: actionModules add: 1")]
        public async Task<IActionResult> CreateActionModule(AddActionModuleDto addActionModuleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _actionModuleServices.CreateActionModule(addActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateActionModule/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: actionModules update: 1")]
        public async Task<IActionResult> UpdateActionModule([FromRoute] int id, EditActionModuleDto editActionModuleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _actionModuleServices.UpdateActionModule(id, editActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteActionModule/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: actionModules delete: 1")]
        public async Task<IActionResult> DeleteActionModule(int id, DeleteActionModuleDto deleteActionModuleDto)
        {
            var response = await _actionModuleServices.DeleteActionModule(id, deleteActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
