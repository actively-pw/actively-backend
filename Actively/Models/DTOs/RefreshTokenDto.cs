using System.ComponentModel.DataAnnotations;

namespace Actively.Models.DTOs
{
	public class RefreshTokenDto
	{
		[Required]
		public string ExpiredToken { get; set; }
		[Required]
		public string RefreshToken { get; set; }
	}
}
