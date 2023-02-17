namespace BE.Data.Dtos.Permission_Use_Menus
{
    public class getPermissionByUserModuleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public int Add { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
        public int Export { get; set; }
    }
}
