using Actively.Models;

namespace Actively.Services.AuthService.Interfaces
{
	public interface ITokenService
	{
		string GenerateToken(User user);
	}
}
