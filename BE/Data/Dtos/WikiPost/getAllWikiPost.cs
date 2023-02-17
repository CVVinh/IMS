using BE.Data.Enum.Wiki;
namespace BE.Data.Dtos.WikiPost
{
    public class getAllWikiPost
    {

        public string title { get; set; }
        public string content { get; set; }
        public Status_Wiki status { get; set; }
        public string note { get; set; }
        public int userCrete { get; set; }
        public DateTime dateCreate { get; set; }
        public int userUpdate { get; set; }
        public DateTime dateUpdate { get; set; }
        public int idCateWiki { get; set; }
    }
}
