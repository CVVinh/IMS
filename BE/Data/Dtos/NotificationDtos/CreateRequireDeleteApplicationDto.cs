namespace BE.Data.Dtos.NotificationDtos
{
    public class CreateRequireDeleteApplicationDTO
    {
        public int? requestUser { get; set; }
        public string? usercode { get; set; }
        public string message { get; set; }
        public string title { get; set; }
    }
}
