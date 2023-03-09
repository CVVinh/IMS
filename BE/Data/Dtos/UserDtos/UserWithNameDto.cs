namespace BE.Data.Dtos.UserDtos
{
    public class UserWithNameDto
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string FullName
        {
            get { return $"{lastName} {firstName}"; }
        }
        public string userCode { get; set; }
    }
}
