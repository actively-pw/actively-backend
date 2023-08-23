using System.ComponentModel.DataAnnotations;

namespace Actively.Models
{
	public class User
	{
		[Key]
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public User(string name, string surname, string email, string password)
		{
			Id = Guid.NewGuid();
			Name = name;
			Surname = surname;
			Email = email;
			Password = password;
		}
	}
}
