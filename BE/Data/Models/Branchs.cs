using MessagePack;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    public class Branchs
    {
        public int idBranch { get; set; }
        public string branchName { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; }
        
        public int isDeleted { get; set; }
    }
}