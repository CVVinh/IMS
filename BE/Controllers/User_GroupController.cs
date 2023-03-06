using BE.Data.Contexts;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.UserDtos;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.RulesServices;
using BE.Services.UserServices;
using BE.shared.Interface;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_GroupController : ControllerBase
    {
        private readonly IUserGroupServices _userGroupServices;
        private readonly IPaginationServices<User_Group> _paginationService;

        public User_GroupController(IUserGroupServices userGroupServices, IPaginationServices<User_Group> paginationService)
        {
            _userGroupServices = userGroupServices;
            _paginationService = paginationService;
        }

        [HttpGet("getAllUserGroupAsync")]
        public async Task<IActionResult> GetAllUserGroupAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _userGroupServices.GetAllUserGroupAsync();
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

        [HttpGet("getUserGroupWithGroupId/{idGroup}")]
        public async Task<IActionResult> GetUserGroupWithGroupId([FromRoute] int idGroup)
        {
            var response = await _userGroupServices.GetUserGroupWithGroupId(idGroup);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("getUserGroupWithUserId/{idUser}")]
        public async Task<IActionResult> GetUserGroupWithUserId([FromRoute] int idUser)
        {
            var response = await _userGroupServices.GetUserGroupWithUserId(idUser);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userGroupServices.CreateUserGroup(userGroupCreatedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateUserGroup/{idUser}/{idGroup}")]
        public async Task<IActionResult> UpdateUserGroup([FromRoute] int idUser, [FromRoute] int idGroup, UserGroupUpdatedDto userGroupUpdatedDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var rules = await _userGroupServices.UpdateUserGroup(idUser, idGroup, userGroupUpdatedDto);
            if (rules._success)
            {
                return Ok(rules);
            }
            return BadRequest(rules);
        }

        [HttpPut("deleteUserGroup")]
        public async Task<IActionResult> DeleteUserGroup([FromQuery] UserGroupDeletedDto userGroupDeletedDto)
        {
            var response = await _userGroupServices.DeleteUserGroup(userGroupDeletedDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
