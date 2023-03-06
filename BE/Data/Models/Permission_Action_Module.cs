using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Permission_Action_Module : BaseEntity
    {
        public int idModule { get; set; }

        public int idAction { get; set; }
    }
}
