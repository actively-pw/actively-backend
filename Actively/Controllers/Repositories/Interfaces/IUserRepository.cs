using Actively.Models;
using Actively.Models.DTOs;

namespace Actively.Controllers.Repositories.Interfaces
{
	/// <summary>
	/// Interface for classes used for database management and queries related to users
	/// </summary>
	public interface IUserRepository
	{
		Task<User?> GetUserByEmailAsync(string email);
		Task<User?> GetUserByIdAsync(Guid id);
		Task<User> RegisterUserAsync(RegisterUserDto registerUserDto, string hashedPassword);

	}
}
