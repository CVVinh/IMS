using System.Net;

namespace BE.Response
{
    public class ApiRespones<T>
    {
        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int ToTalPage { get; set; }
    }
}
