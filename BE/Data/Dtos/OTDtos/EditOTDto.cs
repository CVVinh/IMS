namespace BE.Data.Dtos.OTDtos
{
    public class EditOTDto
    {
        public DateTime Date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string realTime { get; set; }
        public int user { get; set; }
        public int idProject { get; set; }
        public string description { get; set; }
        public int? updateUser { get; set; }

    }
}
