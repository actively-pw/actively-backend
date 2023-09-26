using Actively.Models;
using Actively.Models.DTOs;

namespace Actively.Services.AuthService.Interfaces
{
	public interface ITokenService
	{
		Task<TokensDto> GetTokens(User user, string ipAddress);
	}
}
