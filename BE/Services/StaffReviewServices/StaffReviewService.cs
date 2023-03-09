using AutoMapper;
using AutoMapper.QueryableExtensions;
using BE.Data.Contexts;
using BE.Data.Dtos.StaffReviewDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.StaffReviewServices
{
    public interface IStaffReviewService
    {
        Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReview();
        Task<BaseResponse<StaffReview>> CreateStaffReview(CreateStaffReviewDto staffReviewDto);
    }
    public class StaffReviewService : IStaffReviewService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StaffReviewService(AppDbContext appcontext, IMapper mapper)
        {
            _context = appcontext;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<StaffReviewDto>>> GetAllStaffReview()
        {
            var getAll = await _context.StaffReviews.Include(x => x.ReviewResult).Include(x => x.experiences).ToListAsync();
            if (getAll == null)
                return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", new List<StaffReviewDto>());
       
            var result = _mapper.Map<List<StaffReviewDto>>(getAll);

            return new BaseResponse<List<StaffReviewDto>>(true, "Get All List Staff Review Successfully", result);
        }
        public async Task<BaseResponse<StaffReview>> CreateStaffReview(CreateStaffReviewDto staffReviewDto)
        {
            var map = _mapper.Map<StaffReview>(staffReviewDto);
            _context.StaffReviews.Add(map);
            _context.SaveChanges();
            return new BaseResponse<StaffReview>(true, "Create Staff Review Ticket Successfully", map);
        }
    }
}
