using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.ActionModuleServices;
using BE.Services.PaginationServices;
using BE.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionActionModuleController : ControllerBase
    {
        private readonly IPermissionActionModuleServices _permissionActionModuleServices;
        private readonly IPaginationServices<Permission_Action_Module> _paginationService;

        public PermissionActionModuleController(IPermissionActionModuleServices permissionActionModuleServices, IPaginationServices<Permission_Action_Module> paginationService)
        {
            _permissionActionModuleServices = permissionActionModuleServices;
            _paginationService = paginationService;
        }

        [HttpGet("getAllPermissionActionModule")]
        public async Task<IActionResult> GetAllPermissionActionModuleAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _permissionActionModuleServices.GetAllPermissionActionModuleAsync();
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

        [HttpGet("getPermissionActionModuleWithModuleId/{moduleId}")]
        public async Task<IActionResult> GetPermissionActionModuleWithModuleId([FromRoute] int moduleId)
        {
            var response = await _permissionActionModuleServices.GetPermissionActionModuleWithModuleId(moduleId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionActionModuleWithActionId/{actionId}")]
        public async Task<IActionResult> GetPermissionActionModuleWithActionId([FromRoute] int actionId)
        {
            var response = await _permissionActionModuleServices.GetPermissionActionModuleWithActionId(actionId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionActionModuleServices.CreatePermissionActionModule(addPermissionActionModuleDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUPermissionActionModule/{idModule}/{idAction}")]
        public async Task<IActionResult> UpdateUPermissionActionModule([FromRoute] int idModule, [FromRoute] int idAction, UpdatePermissionActionModuleDto updatePermissionActionModuleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionActionModuleServices.UpdateUPermissionActionModule(idModule, idAction, updatePermissionActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deletePermissionActionModule/{idModule}/{idAction}")]
        public async Task<IActionResult> DeletePermissionActionModule([FromRoute] int idModule, [FromRoute] int idAction, DeletePermissionActionModuleDto deletePermissionActionModuleDto)
        {
            var response = await _permissionActionModuleServices.DeletePermissionActionModule(idModule, idAction, deletePermissionActionModuleDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }

}
