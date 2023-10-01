using Actively.Models;
using Actively.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task<UserRefreshToken> InvalidateRefreshTokenAsync(string ipAddress, JwtSecurityToken jwt, TokensDto tokensDto);
	}
}
