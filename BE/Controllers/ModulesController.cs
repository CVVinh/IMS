using BE.Data.Contexts;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BE.Services.GroupServices;
using BE.Services.PaginationServices;
using BE.Services.ModuleServices;
using BE.Data.Enum;
using BE.Data.Dtos.GruopDtos;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: modules")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleServices _moduleServices;
        private readonly IPaginationServices<Module> _paginationService;

        public ModulesController(IModuleServices moduleServices, IPaginationServices<Module> paginationService)
        {
            _moduleServices = moduleServices;
            _paginationService = paginationService;
        }

        [HttpGet("getListModule")]
        public async Task<IActionResult> GetAllModuleAsync(int? pageIndex, PageSizeEnum pageSizeEnum)
        {
            var response = await _moduleServices.GetAllModuleAsync();
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

        [HttpGet("getModuleById/{id}")]
        public async Task<IActionResult> GetModuleById([FromRoute] int id)
        {
            var response = await _moduleServices.GetModuleById(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("createModule")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: modules add: 1")]
        public async Task<IActionResult> CreateModule(ModuleDtos moduleDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _moduleServices.CreateModule(moduleDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("updateModule/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: modules update: 1")]
        public async Task<IActionResult> UpdateModule([FromRoute] int id, ModuleDtos moduleDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _moduleServices.UpdateModule(id, moduleDtos);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteModule/{id}")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: modules delete: 1")]
        public async Task<IActionResult> DeleteModule([FromRoute] int id)
        {
            var response = await _moduleServices.DeleteModule(id);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("deleteMultiModule")]
        //[Authorize(Roles = "admin")]
        [Authorize(Roles = "modules: modules deleteMulti: 1")]
        public async Task<IActionResult> DeleteMultiModule(List<int> listIdModule)
        {
            var response = await _moduleServices.DeleteMultiModule(listIdModule);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}


