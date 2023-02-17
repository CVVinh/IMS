using BE.Data.Enum.Wiki;

namespace BE.Data.Dtos.WikiPost
{
    public class editWikiPost
    {

        public string title { get; set; }
        public string content { get; set; }
        public Status_Wiki status { get; set; }
        public string note { get; set; }
        public int userUpdate { get; set; }
        public int idCateWiki { get; set; }
    }
}
