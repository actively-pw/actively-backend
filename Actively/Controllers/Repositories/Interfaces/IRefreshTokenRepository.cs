using Actively.Models;
using Actively.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Controllers.Repositories.Interfaces
{
	/// <summary>
	/// Interface for classes used for database management and queries related to refresh tokens
	/// </summary>
	public interface IRefreshTokenRepository
	{
		Task<UserRefreshToken> InvalidateRefreshTokenAsync(string ipAddress, JwtSecurityToken jwt, TokensDto tokensDto);
	}
}
