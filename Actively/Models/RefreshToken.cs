using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFitBook.Models
{
	public class UserRefreshToken
	{
		[Key]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		[ForeignKey("UserId")]
		public virtual User User { get; set; }
		public string Token { get; set; }
		public string RefreshToken { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime ExpirationDate { get; set; }
		[NotMapped]
		public bool isActive
		{
			get
			{
				return ExpirationDate > DateTime.Now;
			}
		}
		public string IpAddress { get; set; }
		public bool IsInvalidated { get; set; }

	}
}
