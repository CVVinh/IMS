using BE.Data.Contexts;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModulesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("addModule")]
        [Authorize(Roles = "permission_group: True module: groups")]
        [Authorize(Roles = "group: Admin")]
        public IActionResult Create(addModuleDtos module)
        {
            try
            {
                var m = new Module
                {

                    nameModule = module.nameModule,
                    note = module.note,
                };

                _context.Add(m);
                _context.SaveChanges();
                return Ok(m);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("updateMoudel")]
        [Authorize(Roles = "permission_group: True module: groups")]
        [Authorize(Roles = "group: Admin")]
        public async Task<ActionResult> updateModule(Module mod)
        {
            try
            {
                var upModules = await _context.modules.FindAsync(mod.id);
                if (upModules == null)
                {
                    return NotFound("Not found");
                }
                upModules.nameModule = mod.nameModule;
                upModules.note = mod.note;
                await _context.SaveChangesAsync();
                return Ok(upModules);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("getListModule")]
        public async Task<IActionResult> getListModule()
        {
            try
            {
                var list = await _context.modules.OrderBy(m => m.idSort).ToListAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("deleteModule/{idModule}")]
        [Authorize(Roles = "permission_group: True module: groups")]
        [Authorize(Roles = "group: Admin")]
        public async Task<IActionResult> deleteModule(int idModule)
        {
            try
            {
                var module = await _context.modules.SingleOrDefaultAsync(p => p.id == idModule);
                if(module == null)
                {
                    return NotFound();
                }
                else
                {
                    module.isDeleted = 1;
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
            

