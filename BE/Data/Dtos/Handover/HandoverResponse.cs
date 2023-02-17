namespace BE.Data.Dtos.Handover
{
    public class HandoverResponse
    {
        public List<Models.Handover> Handover { get; set; } = new List<Models.Handover>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
