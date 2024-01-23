using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Actively.Controllers.Repositories
{
	/// <summary>
	/// Class that contains operations related to database queries about users
	/// </summary>
	public class UserRepository : IUserRepository
	{
		private readonly ActivelyDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserRepository"/> class.
		/// </summary>
		/// <param name="context"></param>
		public UserRepository(ActivelyDbContext context)
        {
            _context = context;
        }

		/// <summary>
		/// Returns user whose e-mail address is equal to provided <c>email</c> 
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public async Task<User?> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		/// <summary>
		/// Returns user whose Id is equal to provided <c>id</c> 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<User?> GetUserByIdAsync(Guid id)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
		}

		/// <summary>
		/// Registers new user
		/// </summary>
		/// <param name="registerUserDto"></param>
		/// <param name="hashedPassword"></param>
		/// <returns></returns>
		public async Task<User> RegisterUserAsync(RegisterUserDto registerUserDto, string hashedPassword)
		{
			var user = new User(
				registerUserDto.Name,
				registerUserDto.Surname,
				registerUserDto.Email,
				hashedPassword);

			await _context.Users.AddAsync(user);
			await _context.SaveChangesAsync();
			return user;
		}

	}
}
