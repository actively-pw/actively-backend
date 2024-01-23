using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Controllers.Repositories
{
	/// <summary>
	/// Class that contains operations related to database queries about refresh tokens
	/// </summary>
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly ActivelyDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
		/// </summary>
		/// <param name="context"></param>
		public RefreshTokenRepository(ActivelyDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Invalidates provided refresh token
		/// </summary>
		/// <param name="ipAddress"></param>
		/// <param name="jwt"></param>
		/// <param name="tokensDto"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public async Task<UserRefreshToken> InvalidateRefreshTokenAsync(string ipAddress, JwtSecurityToken jwt, TokensDto tokensDto)
		{
			// check if it is possible to refresh jwt
			
			if(jwt.ValidTo > DateTime.UtcNow)
			{
				throw new ArgumentException("Jwt has not expired yet");
			}

			var refreshToken = _context.RefreshTokens.Include(r => r.User).FirstOrDefault(r =>
				!r.IsInvalidated &&
				r.Token == tokensDto.Jwt &&
				r.RefreshToken == tokensDto.RefreshToken);

			if (refreshToken is null)
			{
				throw new ArgumentNullException("Invalid token details");
			}

			if(!refreshToken.isActive)
			{
				throw new ArgumentException("Refresh token is expired");
			}

			// update current refresh token

			refreshToken.IsInvalidated = true;
			_context.RefreshTokens.Update(refreshToken);
			await _context.SaveChangesAsync();
			return refreshToken;
		}
	}
}
