namespace BE.Data.Dtos.GruopDtos
{
    public class UpdateGroupDtos
    {
        public int Id { get; set; }
        public string NameGroup { get; set; }
        public string? Discription { get; set; }
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; }
    }   
}
