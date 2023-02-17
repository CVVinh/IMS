using BE.Data.Contexts;

namespace BE.Data.Dtos.LeaveOffDtos
{
    public class CommonHelperDtos
    {
        private readonly AppDbContext _context;

        public CommonHelperDtos(AppDbContext context)
        {
            _context = context;
        }

        // test user Existed 
        public bool userExist(int idUser)
        {
            var project = _context.Users.Where(p => p.id == idUser && p.workStatus == 1).FirstOrDefault();
            if (project is null)
            {
                return false;
            }
            return true;
        }
    }
}
