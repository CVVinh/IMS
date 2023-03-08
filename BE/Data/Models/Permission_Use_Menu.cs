using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Permission_Use_Menu")]
    public class Permission_Use_Menu
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int idModule { get; set; }
        [Required]
        public int IdUser { get; set; }
        [Required]
        public int IdMenu { get; set; }
        [Required]
        public int Add { get; set; }
        [Required]
        public int Update { get; set; }
        [Required]
        public int Delete { get; set; }
        [Required]
        public int DeleteMulti { get; set; }
        [Required]
        public int Confirm { get; set; }
        [Required]
        public int ConfirmMulti { get; set; }
        [Required]
        public int Refuse { get; set; }
        [Required]
        public int AddMember { get; set; }
        [Required]
        public int Export { get; set; }
        public int? userCreated { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? userModified { get; set; }
        public DateTime? dateModified { get; set; }

        public Permission_Use_Menu() { }

        public Permission_Use_Menu(int idModule, int IdUser, int IdMenu, int Add, int Update, int Delete, int DeleteMulti, int Confirm, int ConfirmMulti, int Refuse, int AddMember, int Export, int? userCreated=0)
        {
            this.idModule= idModule;
            this.IdUser = IdUser;
            this.IdMenu = IdMenu;
            this.Add = Add;
            this.Update = Update;
            this.Delete = Delete;
            this.Confirm = Confirm;
            this.ConfirmMulti = ConfirmMulti;
            this.Refuse = Refuse;
            this.AddMember = AddMember;
            this.Export = Export;
            this.userCreated = userCreated;
            this.dateCreated = DateTime.Now;
        }
    }
}
