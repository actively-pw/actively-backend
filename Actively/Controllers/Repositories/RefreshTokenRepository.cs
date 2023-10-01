using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Controllers.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly ActivelyDbContext _context;

		public RefreshTokenRepository(ActivelyDbContext context)
		{
			_context = context;
		}

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
				r.RefreshToken == tokensDto.RefreshToken &&
				r.IpAddress == ipAddress);

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
