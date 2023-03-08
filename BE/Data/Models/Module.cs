using System.ComponentModel.DataAnnotations;
namespace BE.Data.Models
{
    public class Module
    {
        public int id { get; set; }
        public string nameModule { get; set; }
        public string note { get; set; }
        public int isDeleted { get; set; }
        public int idSort { get; set; }

        public ICollection<Permission_Action_Module> permissionActions { get; set; }
    }
}
