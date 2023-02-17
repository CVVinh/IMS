using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Roles")]
    public class Roles
    {
        [Key]
        public int idRole { get; set; }
        public string nameRole { get; set; }
        public string description { get; set; }
        public bool disabled { get; set; }
    }
}
