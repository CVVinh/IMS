using BE.Data.Contexts;
using BE.Data.Models;
using BE.Helpers;

namespace API_LVTN.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext _context;
        private GenericRepository<Devices>? devicesRepository;
        private GenericRepository<Users>? usersRepository;
        private GenericRepository<Permission_Use_Menu>? permission_use_menuRepository;
        private GenericRepository<Permission_Group>? permission_groupRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            GetPermission._context = _context;
        }
        
        public GenericRepository<Devices> DevicesRepository
        {
            get
            {
                if (devicesRepository == null)
                {
                    devicesRepository = new GenericRepository<Devices>(_context);
                }
                return devicesRepository;
            }
        }

        public GenericRepository<Users> UsersRepository
        {
            get
            {
                if (usersRepository == null)
                {
                    usersRepository = new GenericRepository<Users>(_context);
                }
                return usersRepository;
            }
        }

        public GenericRepository<Permission_Use_Menu> PermissionUseMenuRepository
        {
            get
            {
                if (permission_use_menuRepository == null)
                {
                    permission_use_menuRepository = new GenericRepository<Permission_Use_Menu>(_context);
                }
                return permission_use_menuRepository;
            }
        }

        public GenericRepository<Permission_Group> PermissionGroupRepository
        {
            get
            {
                if (permission_groupRepository == null)
                {
                    permission_groupRepository = new GenericRepository<Permission_Group>(_context);
                }
                return permission_groupRepository;
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
