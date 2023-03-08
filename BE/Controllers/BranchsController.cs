using BE.Data.Contexts;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TokenServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: branchs")]
    public class BranchsController : Controller
    {
        private readonly AppDbContext _context;
        
        public BranchsController(AppDbContext context)
        {
            _context = context;
           
        }
        [HttpGet("getListBranch")]
        public ActionResult getListBranch()
        {
            try
            {
                var dsBranch = _context.Branchs.ToList();
                return Ok(dsBranch);

            }
            catch(Exception er)
            {
                return BadRequest(er);
            }
        }
    }
}
