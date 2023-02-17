using System.ComponentModel.DataAnnotations;

namespace BE.Data.Models.Base
{
	public class BaseEntity
	{
		public int id { get; set; }
		public bool isDeleted { get; set; } = false;
		public int? userCreated { get; set; }
		public int? userUpdated { get; set; }
		public int? userDeleted { get; set; }
		public DateTime? dateCreated { get; set; }
		public DateTime? dateUpdated { get; set; }
		public DateTime? dateDeleted { get; set; }
	}
}
