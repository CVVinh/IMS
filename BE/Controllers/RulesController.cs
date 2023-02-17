using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Enum;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.PaidServices;
using BE.shared.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RulesController : ControllerBase
	{
		private readonly IRulesService _rulesService;
		private readonly IPaginationServices<Rules> _paginationService;
		private readonly IWebHostEnvironment _host;
		public RulesController(IRulesService rulesService, IPaginationServices<Rules> paginationService, IWebHostEnvironment host)
		{
			_rulesService = rulesService;
			_paginationService = paginationService;
			_host = host;
		}
		/// <summary>
		/// Get all rules
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSizeEnum"></param>
		/// <returns></returns>
		[HttpGet("getAllRules")]
		public async Task<IActionResult> GetAllRules(int? pageIndex, PageSizeEnum pageSizeEnum)
		{
			var response = await _rulesService.GetAllRulesAsync();
			if (response._success)
			{
				var pageSize = (int)pageSizeEnum;
				var resultPage = await _paginationService.paginationListTableAsync(response._Data, pageIndex, pageSize);
				if (resultPage._success)
				{
					return Ok(resultPage);
				}
				return BadRequest(resultPage);
			}
			return BadRequest(response);
		}
		[HttpGet("GetRulesByIdUser/{id}")]
		public async Task<IActionResult> GetRulesByIdUser([FromRoute] int id)
		{
			var response = await _rulesService.GetRulesWithUserId(id);
			if (response._success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		[HttpPost]
		public async Task<IActionResult> CreateRules([FromForm] AddOrUpdateRulesDTO createRules)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var pathServer = $"{Request.Scheme}://{Request.Host}";
			var response = await _rulesService.CreateRules(createRules, _host.WebRootPath, pathServer);
			if (response._success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		[HttpPut("updateRules/{id}")]
		public async Task<IActionResult> UpdateRules([FromRoute] int id, [FromForm] AddOrUpdateRulesDTO updateRules)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var pathServer = $"{Request.Scheme}://{Request.Host}";
			var rules = await _rulesService.UpdateRules(id, updateRules, _host.WebRootPath, pathServer);
			if (rules._success)
			{
				return Ok(rules);
			}
			return BadRequest(rules);
		}
		[HttpPut("deleteRules/{id}")]
		public async Task<IActionResult> DeleteRules([FromRoute] int id, DeleteRulesDTO userDelete)
		{
			var response = await _rulesService.DeleteRules(id, userDelete);
			if (response._success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		[HttpGet("searchRulesByTitle/{searchTitle}")]
		public async Task<IActionResult> FindRulesByTitleName(string searchTitle)
		{
			var response = await _rulesService.FindRulesByTitle(searchTitle);
			if (response._success)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
	}
}
