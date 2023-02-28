using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.PaidServices
{
    public interface IPaidReasonsService
    {
        Task<BaseResponse<List<PaidReasons>>> GetAllAsync();
        Task<BaseResponse<PaidReasons>> GetById(int paidReasonsId);
    }

    public class PaidReasonServices : IPaidReasonsService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;

        public PaidReasonServices(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<PaidReasons>>> GetAllAsync()
        {
            var success = false;
            var message = "";
            var data = new List<PaidReasons>();
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(s => s.isDeleted == false).OrderBy(s => s.name).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(paidReasons);
                return (new BaseResponse<List<PaidReasons>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<PaidReasons>>(success, message, data));
            }
        }

        public async Task<BaseResponse<PaidReasons>> GetById(int paidReasonsId)
        {
            var success = false;
            var message = "";
            var data = new PaidReasons();
            try
            {
                var paidReasons = await _appContext.PaidReasons.Where(x => x.isDeleted == false && x.id == paidReasonsId).OrderByDescending(x => x.dateCreated).FirstOrDefaultAsync();

                if (paidReasons is null)
                {
                    message = "paidReasonId doesn't exist !";
                    data = null;
                    return new BaseResponse<PaidReasons>(success, message, data);
                }
                success = true;
                data = paidReasons;
                message = "Get data successfully";
                return (new BaseResponse<PaidReasons>(success, message, data));
            }
            catch (Exception ex)
            {
                success = true;
                message = ex.Message;
                return (new BaseResponse<PaidReasons>(success, message, data));
            }
        }


    }
}
