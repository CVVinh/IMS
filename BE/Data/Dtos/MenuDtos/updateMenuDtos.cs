using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.MenuDtos
{
    public class updateMenuDtos
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int idModule { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string view { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public int parent { get; set; }
    }
}
