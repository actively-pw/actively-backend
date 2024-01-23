using Actively.Models;
using Actively.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;

namespace Actively.Services.AuthService.Interfaces
{
	/// <summary>
	/// Interface for classes that allow basic operations on JWTs
	/// </summary>
	public interface ITokenService
	{
		Task<TokensDto> GetTokens(User user, string ipAddress);
		JwtSecurityToken GetJwt(string token);
	}
}
