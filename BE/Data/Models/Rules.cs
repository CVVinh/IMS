using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BE.Data.Models.Base;
using Microsoft.AspNetCore.Http;

namespace BE.Data.Models
{
	public class Rules : BaseEntity
	{
		public string title { get; set; }
		public string? content { get; set; }
		public DateTime? applyDay { get; set; }
		public DateTime? expiredDay { get; set; }
		public string? pathFile { get; set; }
		[NotMapped]
		public IFormFile? formFile { get; set; }
	}
}
