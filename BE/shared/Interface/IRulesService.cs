using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Models;
using BE.Response;

namespace BE.shared.Interface
{
	public interface IRulesService
	{
		Task<BaseResponse<List<Rules>>> GetAllRulesAsync();
		Task<BaseResponse<List<Rules>>> GetRulesWithUserId(int userId);
		Task<BaseResponse<Rules>> CreateRules(AddOrUpdateRulesDTO addRulesDto, string upload,string localServer);
		Task<BaseResponse<Rules>> UpdateRules(int id, AddOrUpdateRulesDTO addRulesDto, string upload, string localServer);
		Task<BaseResponse<Rules>> DeleteRules(int idRules, DeleteRulesDTO deleteRules);
		Task<BaseResponse<List<Rules>>> FindRulesByTitle(string searchTitle);
	}
}
