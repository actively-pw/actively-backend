using Actively.Models;
using Actively.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Services.AuthService.Interfaces
{
	public interface ITokenService
	{
		Task<TokensDto> GetTokens(User user, string ipAddress);
		JwtSecurityToken GetJwt(string token);
	}
}
