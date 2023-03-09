using BE.Data.Dtos.StaffReviewDtos;
using BE.Services.StaffReviewServices;
using Microsoft.AspNetCore.Mvc;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffReviewController : ControllerBase
    {
        private readonly IStaffReviewService _staffReviewService;

        public StaffReviewController(IStaffReviewService staffReviewService)
        {
            _staffReviewService = staffReviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaffReview()
        {
          var response = await _staffReviewService.GetAllStaffReview(); 
            if(response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);    
        }
        [HttpPost]
        public async Task<IActionResult> CreateStaffReview(CreateStaffReviewDto createStaffReviewDto)
        {
            var response = await _staffReviewService.CreateStaffReview(createStaffReviewDto);
            if (response._success)
            {
                return Ok(response);
            }
            return BadRequest(response);    
        }
    }
}
