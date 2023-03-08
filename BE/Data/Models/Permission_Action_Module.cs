using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Permission_Action_Module : BaseEntity
    {
        public int moduleId { get; set; }
        public Module module { get; set; }

        public int actionModuleId { get; set; }

        public Action_Module actionModule { get; set; }
    }
}
