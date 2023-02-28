using BE.Data.Contexts;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE.Data.Dtos.MenuDtos;
using BE.Data.Dtos.Handover;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
      
        private readonly AppDbContext _context;
       
        public MenuController(AppDbContext context)
        {
            _context = context;
          
        }

        [HttpGet]
        [Route("getListMenuModule")]
        public async Task<IActionResult> getListMenuModule()
        {
            try
            {
                var menuModule = await _context.modules.OrderBy(m => m.idSort).ToListAsync();
                return Ok(menuModule);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getListMenuParent")]
        public async Task<ActionResult> getListMenuParent()
        {
            try
            {
                var menuParent = await (from menu in _context.Menus.Where(a => a.parent == 0)
                                   from modules in _context.modules.Where(a => a.id == menu.idModule).DefaultIfEmpty()
                                   select new
                                   {
                                       action = menu.action,
                                       controller = menu.controller,
                                       icon = menu.icon,
                                       id = menu.id,
                                       idModule = menu.idModule,
                                       isDeleted = menu.isDeleted,
                                       parent = menu.parent,
                                       title = menu.title,
                                       view = menu.view, 
                                       idSort = modules.idSort,
                                   }).OrderBy(a => a.idSort).ToListAsync();
                
                return Ok(menuParent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        


        [HttpGet("getListMenu")]
        public async Task<ActionResult> getListMenu()
        {
            try
            {
                var menu = await _context.Menus
                    .Join( _context.modules,
                    menu1 => menu1.idModule, modules => modules.id,
                    (menu1, modules) => new { menu1, modules })
                    .Select(x => new {
                        x.menu1,
                        x.modules.nameModule,
                        x.modules.note,
                    })
                    .ToListAsync();
                if (menu.Count() != 0)
                {
                    return Ok(menu);
                }
                return NotFound("Khong co du lieu");
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("getlistMenubyIdModoule/{ad}")]
        public async Task<ActionResult> getlistMenubyIdModoule(int ad)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == ad && x.isDeleted == 0).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("addMenu")]
        public async Task<IActionResult> addMenu(addMenuDtos request)
        {
            try
            {
               
                var menu = new Menu();
                //menu.id = request.id;
                menu.title = request.title;
                menu.idModule = request.idModule;
                menu.controller = request.controller;
                menu.view = request.view;
                menu.icon = request.icon;
                menu.action = request.action;
                menu.parent = request.parent;
                menu.isDeleted = 0;
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

               

                return Ok("Sucess");

                
                //return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("getlistmenuID/{id}")]
        public async Task<ActionResult> getlistmenuID(int id)
        {
            try
            {
                var menuParent = await _context.Menus.FirstOrDefaultAsync(x=>x.id==id);

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetlistMenuByIdModule/{idmodule}")]
        public async Task<ActionResult> GetlistMenuByIdModule(int idmodule)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == idmodule).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetlistMenuByIdModuleedit/{idmodule}/{idmenu}")]
        public async Task<ActionResult> GetlistMenuByIdModuleedit(int idmodule,int idmenu)
        {
            try
            {
                var menuParent = await _context.Menus.Where(x => x.idModule == idmodule && x.id != idmenu && x.isDeleted == 0).ToListAsync();

                return Ok(menuParent);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut("updateMenu")]
        public async Task<ActionResult> updateMenu(updateMenuDtos requests)
        {
            try
            {
                var acc = await _context.Menus.FindAsync(requests.id);
                if (acc == null)
                {
                    return NotFound();
                }
                acc.title = requests.title;
                acc.controller = requests.controller;
                acc.view = requests.view;
                acc.action = requests.action;
                acc.parent = requests.parent;
                acc.icon = requests.icon;
                acc.idModule = requests.idModule;
                await _context.SaveChangesAsync();
                return Ok("success");
            }catch(Exception e)
            {
                return BadRequest(e);
            }
            
        }

        [HttpPut]
        [Route("deleteMenu/{idmenu}")]
        public async Task<IActionResult> deleteMenu(int idmenu)
        {
            try
            {

                var menu = await _context.Menus.SingleOrDefaultAsync(p => p.id == idmenu);
              
                if (menu == null)
                {
                    return NotFound();
                }
                else
                {
                    var child = await _context.Menus.Where(pa => pa.parent == idmenu && pa.isDeleted == 0).ToListAsync();
                    if (child.Count != 0)
                    {
                        return BadRequest("Not found");
                    }
                    else
                    {
                        menu.isDeleted = 1;
                        _context.SaveChanges();
                        return Ok("success");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        

        [HttpGet]
        [Route("getMenuByModule")]
        // Authorize
        public async Task<IActionResult> GetMenuByModule(int moduleId)
        {
            try
            {
                return Ok(await _context.Menus.AsNoTracking().Where(u => u.idModule == moduleId).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
