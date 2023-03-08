namespace BE.Data.Dtos.UserDtos
{
    public class UserDto
    {
        public int id { get; set; }
        public string userCode { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string? phoneNumber { get; set; }
        public DateTime? dOB { get; set; }
        public byte? gender { get; set; }
        public string? address { get; set; }
        public string? university { get; set; }
        public int? yearGraduated { get; set; }
        public string email { get; set; }
        public string? skype { get; set; }
        public byte workStatus { get; set; }
        public DateTime dateStartWork { get; set; }
        public DateTime? dateLeave { get; set; }
        public byte? maritalStatus { get; set; }
        public string? reasonResignation { get; set; }
        public string identitycard { get; set; }
        public byte isDeleted { get; set; }
        public int IdGroup { get; set; }
        public string FullName
        {
            get { return $"{lastName} {firstName}"; }
        }
    }
}
