using Actively.Models;
using Actively.Models.DTOs;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<User?> GetUserByEmailAsync(string email);
		Task<User> RegisterUserAsync(RegisterUserDto registerUserDto, string hashedPassword);

	}
}
