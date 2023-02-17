using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
	[Table("Users")]
	public class Users
	{
		public int id { get; set; }
		public string userCode { get; set; }
		public string userPassword { get; set; }
		public int? userCreated { get; set; }
		public DateTime? dateCreated { get; set; }
		public int? userModified { get; set; }
		public DateTime? dateModified { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string? phoneNumber { get; set; }
		public DateTime? dOB { get; set; }
		public byte? gender { get; set; }
		public string? address { get; set; }
		public string? university { get; set; }
		public int? yearGraduated { get; set; }
		public string email { get; set; }
		public string? emailPassword { get; set; }
		public string? skype { get; set; }
		public string? skypePassword { get; set; }
		public byte workStatus { get; set; }
		public DateTime dateStartWork { get; set; }
		public DateTime? dateLeave { get; set; }
		public byte? maritalStatus { get; set; }
		public string? reasonResignation { get; set; }
		public string identitycard { get; set; }
		public byte isDeleted { get; set; }
		public string? refreshToken { get; set; }
		public DateTime? refreshTokenExpiryTime { get; set; }
		public int IdGroup { get; set; }

		//Date: 8/2/2023
		//Modifile: add field fullName in table User
		[NotMapped]
		public string FullName
		{
			get { return $"{firstName} {lastName}"; }
		}
    }
}