using BE.Data.Contexts;
using BE.Data.Dtos.RoleDtos;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {   
        #region Property
        private readonly AppDbContext _appdbContext;
        public RolesController(AppDbContext appDbContext)
        {
            _appdbContext = appDbContext;
        }
        #endregion

        #region Methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roles>>> GetAll()
        {
            return await _appdbContext.roles.ToListAsync();
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<Roles>>> GetID(int id)
        {
            var role = await _appdbContext.roles.Where(d => d.idRole == id).SingleOrDefaultAsync();
            if(role == null)
                return NotFound();
            return Ok(role);
        }
        [HttpGet]
        [Route("getRoleDropdown")]
        public async Task<ActionResult<IEnumerable<RolesDropdown>>> getRolesDropdown()
        {
            var roles = await _appdbContext.roles.ToListAsync();
            var rolesDropdown = new List<RolesDropdown>();
            foreach (var role in roles)
                rolesDropdown.Add(new RolesDropdown(role));
            return Ok(rolesDropdown);
        }
        #endregion
    }
}
