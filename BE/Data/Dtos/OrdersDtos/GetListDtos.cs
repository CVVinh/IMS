using BE.Data.Models;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Dtos.OrdersDtos
{
    public class GetListDtos
    {
        [Required]
        public int idOrder { get; set; }
        public int idBranch { get; set; }
        public int idDevice { get; set; }
        public int amount { get; set; }
        public DateTime dateCreated { get; set; }
        public int userCreated { get; set; }
        public DateTime dateUpdated { get; set; }
        public int userUpdated { get; set; }
        public int isDeleted { get; set; }
        public string note { get; set; }
        public int statusOrder { get; set; }
        public int statusDevice { get; set; }
        [Required]
        public string DeviceName { get; set; }
        public string branchName { get; set; }

    }
}
