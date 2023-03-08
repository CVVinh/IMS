using BE.Data.Dtos.BlockingWebDto;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Models;
using BE.Response;

namespace BE.shared.Interface
{
    public interface IBlockingWebService
    {
        Task<BaseResponse<BlockingWeb>> AddBlockingWebAsync(AddBlockingWebDto addBlocking);
        Task<BaseResponse<List<BlockingWeb>>> GetAllBlockWebAsync();
    }
}
