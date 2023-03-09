using BE.Data.Contexts;
using BE.Data.Dtos.WikiCateDtos;
using BE.Data.Dtos.WikiDtos;
using BE.Data.Models;
using BE.Services.Wiki;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [ApiController]
    [Route("api/Wiki_Categories")]
    [Authorize(Roles = "permission_group: True module: wikiCates")]
    public class Wiki_CateController : Controller
    {
        #region Property
        private readonly AppDbContext _appContext;
        private readonly IWikiCategogyService _WikiCateServices;
        #endregion
        #region Constructor
        public  Wiki_CateController(AppDbContext appContext, IWikiCategogyService wikiCategogyService)
        {
            _appContext = appContext;
            _WikiCateServices = wikiCategogyService;
        }
        #endregion

        //POST add new wiki categogy
        [HttpPost("addNewWikiCate")]
        //[Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "modules: wikiCates add: 1")]
        public async Task<IActionResult> addNewWikiCate(addWiki_Categogy addWiki_Categogy)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var newWikiCate = await _WikiCateServices.addNewWikiCateAsync(addWiki_Categogy);
                if (newWikiCate._success)
                {
                    return Ok(newWikiCate);
                }

                return BadRequest(newWikiCate);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
        //GET get WikiCateById
        [HttpGet("getWikiCateById/{ID}")]
        public async Task<IActionResult> getWikiCateById(int ID)
        {
            try
            {   
                var results = await _WikiCateServices.getWikiCateById(ID);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //GET get AllWikiCate 
        [HttpGet("getAllCate")]
        public async Task<IActionResult> getAllWikiCate()
        {
            try
            {
                var results = await _WikiCateServices.getAllWikiCate();

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

        //PUT put WikiCate
        [HttpPut("editWikiCate/{ID}")]
        //[Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "modules: wikiCates update: 1")]
        public async Task<IActionResult> editWikiCate(editWikiCate editWikiCate, int ID)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var results = await _WikiCateServices.editWikiCate(editWikiCate, ID);

                if (results._success)
                {
                    return Ok(results);
                }

                return BadRequest(results);

            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //DELETE delete WikiCate
        [HttpDelete("deleteWikiCate/{ID}")]
        //[Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "modules: wikiCates delete: 1")]
        public async Task<IActionResult> deleteWikiCate(int ID)
        {
            try
            {
                var results = await _WikiCateServices.deleteWikiCate(ID);

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
