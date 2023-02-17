using BE.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BE.Data.Dtos.OrdersDtos
{
   
    public class GetOrderDtos
    {
        private readonly AppDbContext _context;

        public GetOrderDtos(AppDbContext context)
        {
            _context = context;
        }

    }
}
