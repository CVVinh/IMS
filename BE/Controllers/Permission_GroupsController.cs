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
    [Authorize(Roles = "permission_group: True module: permissionGroups")]
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

        [HttpGet("getPermissionGroupWithGroupIdAcess/{groupId}")]
        public async Task<IActionResult> GetPermissionGroupWithGroupIdAcess([FromRoute] int groupId)
        {
            var response = await _permissionGroupServices.GetPermissionGroupWithGroupIdAcess(groupId);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createPermissionGroup")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: permissionGroups add: 1")]
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

        [HttpPut("updatePermissionGroup/{idGroup}/{idModule}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: permissionGroups update: 1")]
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

        [HttpPut("updateMultiPermissionGroup/{idGroup}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: permissionGroups update: 1")]
        public async Task<IActionResult> UpdateMultiPermissionGroup([FromRoute] int idGroup, List<ChangePermissionGroupDto> changePermissionGroupDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _permissionGroupServices.UpdateMultiPermissionGroup(idGroup, changePermissionGroupDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpDelete("deletePermissionGroup/{idGroup}/{idModule}")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: permissionGroups delete: 1")]
        public async Task<IActionResult> DeletePermissionGroup([FromRoute] int idGroup, [FromRoute] int idModule)
        {
            var response = await _permissionGroupServices.DeletePermissionGroup(idGroup, idModule);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("deleteMultiPermissionGroup")]
        [Authorize(Roles = "admin")]
        [Authorize(Roles = "module: permissionGroups updateMulti: 1")]
        public async Task<IActionResult> DeleteMultiPermissionGroup(List<PermissionGroupRequestDto> permissionGroupRequestDto)
        {
            var response = await _permissionGroupServices.DeleteMultiPermissionGroup(permissionGroupRequestDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
