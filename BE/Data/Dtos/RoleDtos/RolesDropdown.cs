using BE.Data.Models;

namespace BE.Data.Dtos.RoleDtos
{
    public class RolesDropdown
    {
        public int idRole { get; set; }
        public string roleName { get; set; }
        public RolesDropdown(Roles role)
        {
            idRole = role.idRole;
            roleName = role.nameRole;
        }
    }
}
