namespace BE.Data.Models
{
    public class Member_Project
    {
        public int Id { get; set; }
        public bool isDeleted { get; set; } 
        public DateTime createDate { get; set; }
        public int createUser { get; set; }
        public int member { get; set; }
        public int idProject { get; set; } 
    }
}
