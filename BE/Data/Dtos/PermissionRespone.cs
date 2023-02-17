namespace BE.Data.Dtos
{
    public class PermissionResponse
    {
        public bool Get { get; set; }
        public bool Add { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Export { get; set; }
    }
}
