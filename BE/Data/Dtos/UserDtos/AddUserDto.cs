using BE.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UsersDTO
{
    public class AddUserDto
    {

        [Required]
        public string userCode { get; set; }
        public string userPassword { get; set; }
        public int userCreated { get; set; }
        public DateTime dateCreated { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? dOB { get; set; }
        [Range(1, 3)]
        public byte? gender { get; set; }
        public string? address { get; set; }
        public string? university { get; set; }
        [Range(2000, 3000)]
        public int? yearGraduated { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string? emailPassword { get; set; }
        public string? skype { get; set; }
        public string? skypePassword { get; set; }
        [Range(1, 3)]
        public byte workStatus { get; set; }
        public DateTime dateStartWork { get; set; }
        [Range(1, 3)]
        public byte? maritalStatus { get; set; }
        public string? reasonResignation { get; set; }
        public string identitycard { get; set; } 


        [Range(0, 1)]
        public byte isDeleted { get; set; }
    }

}
