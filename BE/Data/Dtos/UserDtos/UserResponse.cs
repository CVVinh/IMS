namespace BE.Data.Models
{
    public class UserResponse
    {
        public List<Users> Users { get; set; } = new List<Users>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
