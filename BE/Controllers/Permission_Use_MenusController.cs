using BE.Data.Contexts;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Permission_Use_MenusController : ControllerBase
    {
        private readonly AppDbContext _context;
        public Permission_Use_MenusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getPermissionByUser")]
        [Authorize(Roles = "1")]
        public ActionResult<IEnumerable<getPermissionByUserModuleDto>> getPermissionByUser(int userId)
        {
            try
            {
                var getPermission = (from permission in _context.Permission_Use_Menus
                                     join menu in _context.Menus on permission.IdMenu equals menu.id
                                     join module in _context.modules on menu.idModule equals module.id
                                     where (permission.IdUser == userId)
                                     select new List<getPermissionByUserModuleDto>
                                     {
                                         new getPermissionByUserModuleDto
                                         {
                                             Id = permission.Id,
                                             UserId = permission.IdUser,
                                             ModuleId = menu.idModule,
                                             Add = permission.Add,
                                             Update = permission.Update,
                                             Delete = permission.Delete,
                                             Export = permission.Export
                                         }
                                     });
                if (getPermission != null)
                {
                    return Ok(getPermission);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("addPermissionByGroup")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> addPermissionByGroup(int idGroup, int idUserAdd, string[][] data)
        {
            try
            {
                var query = await _context.Users.Where(x => x.IdGroup == idGroup).ToListAsync();
                if (query.Count() != 0)
                {
                    foreach (var item in query)
                    {
                        for (int i = 0; i < data.Length; i++)
                        {
                            var permission = await _context.Permission_Use_Menus.SingleOrDefaultAsync(x => x.IdUser == item.id && x.idModule == int.Parse(data[i][1]));
                            if (permission != null)
                            {
                                permission.IdMenu = int.Parse(data[i][0]);
                                permission.idModule = int.Parse(data[i][1]);
                                permission.Add = int.Parse(data[i][2]);
                                permission.Update = int.Parse(data[i][3]);
                                permission.Delete = int.Parse(data[i][4]);
                                permission.Export = int.Parse(data[i][5]);
                                permission.userModified = idUserAdd;
                                permission.dateModified = DateTime.UtcNow;
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                var ps = new Permission_Use_Menu();
                                ps.IdUser = item.id;
                                ps.IdMenu = int.Parse(data[i][0]);
                                ps.idModule = int.Parse(data[i][1]);
                                ps.Add = int.Parse(data[i][2]);
                                ps.Update = int.Parse(data[i][3]);
                                ps.Delete = int.Parse(data[i][4]);
                                ps.Export = int.Parse(data[i][5]);
                                ps.userCreated = idUserAdd;
                                ps.dateCreated = DateTime.UtcNow;
                                ps.userModified = idUserAdd;
                                ps.dateModified = DateTime.UtcNow;
                                _context.Permission_Use_Menus.Add(ps);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    return Ok();
                }
                return NotFound("No data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("addPermissionByUser")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> addPermissionByUser(int idUser, int idUserAdd, string[][] data)
        {
            try
            {
                var query = await _context.Users.SingleOrDefaultAsync(x => x.id == idUser);
                if (query != null)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        var permission = await _context.Permission_Use_Menus.SingleOrDefaultAsync(x => x.IdUser == query.id && x.idModule == int.Parse(data[i][1]));
                        if (permission != null)
                        {
                            permission.IdMenu = int.Parse(data[i][0]);
                            permission.idModule = int.Parse(data[i][1]);
                            permission.Add = int.Parse(data[i][2]);
                            permission.Update = int.Parse(data[i][3]);
                            permission.Delete = int.Parse(data[i][4]);
                            permission.Export = int.Parse(data[i][5]);
                            permission.userModified = idUserAdd;
                            permission.dateModified = DateTime.UtcNow;
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var ps = new Permission_Use_Menu();
                            ps.IdUser = idUser;
                            ps.IdMenu = int.Parse(data[i][0]);
                            ps.idModule = int.Parse(data[i][1]);
                            ps.Add = int.Parse(data[i][2]);
                            ps.Update = int.Parse(data[i][3]);
                            ps.Delete = int.Parse(data[i][4]);
                            ps.Export = int.Parse(data[i][5]);
                            ps.userCreated = idUserAdd;
                            ps.dateCreated = DateTime.UtcNow;
                            ps.userModified = idUserAdd;
                            ps.dateModified = DateTime.UtcNow;
                            _context.Permission_Use_Menus.Add(ps);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return Ok();
                }
                return NotFound("No data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("getPermission_Use_Menu_ByIdUser")]
        public async Task<IActionResult> getPermission_Use_Menu_ByIdUser(int idUser)
        {
            try
            {
                var data = await _context.Permission_Use_Menus.Where(x=>x.IdUser == idUser).ToListAsync();
                return Ok(data);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
