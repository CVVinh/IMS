namespace BE.Data.Dtos.UsersDTO
{
    public class ApiResponseDto
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public object Token { get; set; }
    }
}
