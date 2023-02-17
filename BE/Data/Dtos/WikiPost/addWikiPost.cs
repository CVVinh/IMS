using BE.Data.Enum.Wiki;
namespace BE.Data.Dtos.WikiPost
{
    public class addWikiPost
    {

        public string title { get; set; }
        public string content { get; set; }
        public Status_Wiki status { get; set; }
        public string note { get; set; }
        public int userCrete { get; set; }
       
       
        public int idCateWiki { get; set; }
    }
}
