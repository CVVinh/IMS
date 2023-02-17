using BE.Data.Contexts;
using BE.Data.Dtos.WikiPost;
using BE.Services.Wiki;
using BE.Services.WikiPost;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Wiki_PostController : ControllerBase
    {

        #region Property
        private readonly AppDbContext _appContext;
        private readonly IWikiPostService _WikiPostServices;
        #endregion
        #region Constructor
        public Wiki_PostController(AppDbContext appContext, IWikiPostService wikiPostService)
        {
            _appContext = appContext;
            _WikiPostServices = wikiPostService;
        }
        #endregion

        //POST add new wiki post
        [HttpPost("addNewWikiPost")]
        public async Task<IActionResult> addNewWikiPost(addWikiPost addWikiPost)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newWikiPost = await _WikiPostServices.addNewWikiPostAsync(addWikiPost);
                if (newWikiPost._success)
                {
                    return Ok(newWikiPost);
                }

                return BadRequest(newWikiPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //GET get WikiPostById
        [HttpGet("getWikiPostById/{ID}")]
        public async Task<IActionResult> getWikiPostById(int ID)
        {
            try
            {
                var results = await _WikiPostServices.getWikiPostById(ID);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //GET get WikiPostByIdCate
        [HttpGet("getWikiPostByIdCate/{IDCate}")]
        public async Task<IActionResult> getWikiPostByIDCate(int IDCate)
        {
            try
            {
                var results = await _WikiPostServices.getAllWikiPostByIDCate(IDCate);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //GET get AllWikiPost
        [HttpGet("getAllWikiPost")]
        public async Task<IActionResult> getAllWikiPost()
        {
            try
            {
                var results = await _WikiPostServices.getAllWikiPost();

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //PUT put WikiPost
        [HttpPut("editWikiPost/{ID}")]
        public async Task<IActionResult> editWikiPost(editWikiPost editWikiPost, int ID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var results = await _WikiPostServices.editWikiPost(editWikiPost, ID);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //DELETE delete WikiPost
        [HttpDelete("deleteWikiPost/{ID}")]
        public async Task<IActionResult> deleteWikiPost(int ID)
        {
            try
            {
                var results = await _WikiPostServices.deleteWikiPost(ID);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
