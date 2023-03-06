namespace BE.Data.Dtos.Permission_Use_Menus
{
    public class PermissionUserMenuAddDto
    {
        public int idModule { get; set; }
        public int IdUser { get; set; }
        public int IdMenu { get; set; }
        public int Add { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
        public int DeleteMulti { get; set; }
        public int Confirm { get; set; }
        public int ConfirmMulti { get; set; }
        public int Refuse { get; set; }
        public int AddMember { get; set; }
        public int Export { get; set; }
        public int userCreated { get; set; }
    }
}
