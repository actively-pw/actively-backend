using Actively.Models;
using Actively.Services.AuthService.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Actively.Services.AuthService
{
	public class TokenService
	{
		private readonly JwtConfig _jwtConfig;

		public TokenService(IOptions<JwtConfig> jwtConfig)
		{
			_jwtConfig = jwtConfig.Value;
		}
		public string GenerateToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Issuer = _jwtConfig.Issuer,
				Audience = _jwtConfig.Audience,
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				}),
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key),
					SecurityAlgorithms.HmacSha256Signature),
				Expires = DateTime.UtcNow.AddSeconds(_jwtConfig.LifetimeInSeconds)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
