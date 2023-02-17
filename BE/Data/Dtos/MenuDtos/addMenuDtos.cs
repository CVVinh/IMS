using System.ComponentModel.DataAnnotations;
namespace BE.Data.Dtos.MenuDtos
{
    public class addMenuDtos
    {
       
        public int idModule { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string view { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public int parent { get; set; }
    }
}
