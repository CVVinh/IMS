using BE.Data.Dtos.MemberProjectDtos;
using BE.Services.MemberProjectServices;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/memberProject")]
    public class MemberProjectController : Controller
    {
        private readonly IMemberProjectServices _memberProjectServices;
        //private readonly ILogger<MemberProjectController> _logger;
        public MemberProjectController(IMemberProjectServices memberProjectServices)
        {
            _memberProjectServices = memberProjectServices;
            //_logger = logger;
        }

        [HttpGet("getAllMemberProjectInProject/{idProject}")]
        public async Task<IActionResult> getAllMemberProjectByIdProject(int idProject)
        {
            var result = await _memberProjectServices.GetMembersByIdProjectAsync(idProject);
            if(result._success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("getMemberProjectById/{id}")]
        public async Task<IActionResult> getMemberById(int id)
        {
            var result = await _memberProjectServices.GetMemberByIdAsync(id);
            if (result._success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("getMemberByIdUserAtProject/{idUser}/{idProject}")]
        public async Task<IActionResult> getMemberByIdUserAtProject(int idUser, int idProject)
        {
            var result = await _memberProjectServices.GetMemberByIdUserAtProjectAsync(idUser,idProject);
            if (result._success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("addMemberAtProject")]
        public async Task<IActionResult> addNewMember(AddMemberDto addMemberDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _memberProjectServices.AddNewMemberAsync(addMemberDto);
            if(!result._success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("deleteMemberById/{id}")]
        public async Task<IActionResult> deleteMemberById(int id)
        {
            var result = await _memberProjectServices.DeleteMemberAsync(id);
            if(!result._success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("deleteMemberInProject/{idMember}/{idProject}")]
        public async Task<IActionResult> deleteMemberByIdProject(int idMember,int idProject)
        {
            var result = await _memberProjectServices.DeleteMemberInProjectAsync(idMember,idProject);
            if (!result._success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
