using BE.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpersController : Controller
    {
        //private readonly UnitOfWork _unitOfWork;
        //private readonly AppDbContext _appDbContext;
        //private readonly IMapper _mapper;
        //private readonly int IdModule = 37;

        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Ok();
        }

        [HttpGet("GetIdUserFromToken")]
        public ActionResult GetIdUserFromToken()
        {
            var idUser = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("Id", StringComparison.InvariantCultureIgnoreCase));
            var das = TokenHelper.GetUserId(User);
            return Ok();
        }
    }
}
