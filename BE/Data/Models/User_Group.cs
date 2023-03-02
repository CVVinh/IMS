using BE.Data.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Models
{
    public class User_Group : BaseEntity
    {
        public int id { get; set; }
        public int idUser { get; set; }
        public int idGroup { get; set; }
    }
}
