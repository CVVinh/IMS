using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.ActionModuleDtos
{
    public class AddActionModuleDto
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public int userCreated { get; set; }
    }
}
