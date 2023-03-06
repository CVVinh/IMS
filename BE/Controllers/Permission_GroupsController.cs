using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.GroupServices;
using BE.Services.PaginationServices;
using BE.Services.UserServices;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Permission_GroupsController : ControllerBase
    {
        private readonly IPermissionGroupServices _permissionGroupServices;
        private readonly IPaginationServices<Permission_Group> _paginationService;

        public Permission_GroupsController(IPermissionGroupServices permissionGroupServices, IPaginationServices<Permission_Group> paginationService)
        {
            _permissionGroupServices = permissionGroupServices;
            _paginationService = paginationService;
        }

        [HttpGet("getPermissionGroup")]
        public async Task<IActionResult> GetAllPermissionGroupAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _permissionGroupServices.GetAllPermissionGroupAsync();
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
        

        [HttpGet("getPermissionGroup_By_IdGroup/{groupId}")]
        public async Task<IActionResult> GetPermissionGroupWithGroupId([FromRoute] int groupId)
        {
            var response = await _permissionGroupServices.GetPermissionGroupWithGroupId(groupId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getPermissionGroup_By_IdModule/{moduleId}")]
        public async Task<IActionResult> GetPermissionGroupWithModuleId([FromRoute] int moduleId)
        {
            var response = await _permissionGroupServices.GetPermissionGroupWithModuleId(moduleId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("decentralization_Group")]
        public async Task<IActionResult> CreatePermissionGroup(List<PermissionGroupDto> permissionGroupDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionGroupServices.CreatePermissionGroup(permissionGroupDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdatePermissionGroup/{idGroup}/{idModule}")]
        public async Task<IActionResult> UpdatePermissionGroup([FromRoute] int idGroup, [FromRoute] int idModule, PermissionGroupDto permissionGroupDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionGroupServices.UpdatePermissionGroup(idGroup, idModule, permissionGroupDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete("deletePermissionGroup/{idGroup}/{idModule}")]
        public async Task<IActionResult> DeletePermissionGroup([FromRoute] int idGroup, [FromRoute] int idModule)
        {
            var response = await _permissionGroupServices.DeletePermissionGroup(idGroup, idModule);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }



    }
}
