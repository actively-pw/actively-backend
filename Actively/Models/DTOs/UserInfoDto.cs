namespace Actively.Models.DTOs
{
	public class UserInfoDto
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public UserInfoDto(User user)
		{
			Name = user.Name;
			Surname = user.Surname;
			Email = user.Email;
		}
	}
}
