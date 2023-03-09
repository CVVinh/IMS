using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UserDtos
{
    public class UpdateProfileDto
    {
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; } = DateTime.Now;
        public string firstName { get; set; }
        public string lastName { get; set; }
        [Phone]
        public string phoneNumber { get; set; }
        public DateTime? dOB { get; set; }
        [Range(1, 3)]
        public byte gender { get; set; }
        public string? address { get; set; }
        public string? university { get; set; }
        [Range(1980, 2100)]
        public int? yearGraduated { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string? skype { get; set; }
        [Range(1, 3)]
        public byte workStatus { get; set; }
        public DateTime? dateLeave { get; set; }
        [Range(1, 3)]
        public byte? maritalStatus { get; set; }
        public string? reasonResignation { get; set; }
        public string identitycard { get; set; }
        [Range(1, 7)]
        public int IdGroup { get; set; }
        public DateTime dateStartWork { get; set; }
        public IFormFile? formFileAvatar { get; set; }
    }
}
