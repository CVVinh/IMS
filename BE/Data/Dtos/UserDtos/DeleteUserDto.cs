namespace BE.Data.Dtos.UserDtos
{
    public class DeleteUserDto
    {
        public byte isDeleted { get; set; }
        public int userModified { get; set; }
        public DateTime dateModified { get; set; }
    }
}
