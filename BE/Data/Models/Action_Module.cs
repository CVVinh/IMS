using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Action_Module : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }

        public ICollection<Permission_Action_Module> permissionActions { get; set; }
    }
}
