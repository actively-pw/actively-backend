using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Actively.Controllers.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly ActivelyDbContext _context;
        public UserRepository(ActivelyDbContext context)
        {
            _context = context;
        }

		public async Task<User?> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

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
