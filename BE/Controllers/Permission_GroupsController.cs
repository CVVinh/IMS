using BE.Data.Contexts;
using BE.Data.Models;
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
        private readonly AppDbContext _context;
        public Permission_GroupsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("decentralization_Group")]
        public async Task<IActionResult> decentralization_Group(int idGroup, string[][] data)
        {
            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int a = int.Parse(data[i][0]);
                    var permission = await _context.Permission_Groups.SingleOrDefaultAsync(x => x.IdGroup == idGroup && x.IdModule == int.Parse(data[i][0]));
                    if (permission != null)
                    {
                        if(int.Parse(data[i][1]) == 0)
                        {
                            permission.Access = false;
                            await _context.SaveChangesAsync();
                        }
                        if(int.Parse(data[i][1]) == 1)
                        {
                            permission.Access = true;
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        var permission_group = new Permission_Group();
                        permission_group.IdGroup = idGroup;
                        permission_group.IdModule = int.Parse(data[i][0]);
                        if (int.Parse(data[i][1]) == 0)
                        {
                            permission_group.Access = false;
                            await _context.SaveChangesAsync();
                        }
                        if (int.Parse(data[i][1]) == 1)
                        {
                            permission_group.Access = true;
                            await _context.SaveChangesAsync();
                        }
                        _context.Permission_Groups.Add(permission_group);
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("getPermissionGroup")]
        public async Task<IActionResult> getPermissionGroup()
        {
            try
            {
                var data = await _context.Permission_Groups.ToListAsync();
                if (data.Count() != 0)
                {
                    return Ok(data);
                }
                return NotFound("No data");
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("getPermissionGroup_By_IdGroup")]
        public async Task<IActionResult> getPermissionGroup_By_IdGroup(int idGroup)
        {
            try
            {
                var data = await _context.Permission_Groups.Where(x=>x.IdGroup == idGroup).ToListAsync();
                if (data != null)
                {
                    return Ok(data);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
