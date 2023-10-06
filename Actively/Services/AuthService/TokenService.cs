using Actively.Context;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.AuthService.Configuration;
using Actively.Services.AuthService.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Actively.Services.AuthService
{
	public class TokenService : ITokenService
	{
		private readonly JwtConfig _jwtConfig;
		private readonly ActivelyDbContext _context;

		public TokenService(IOptions<JwtConfig> jwtConfig, ActivelyDbContext context)
		{
			_jwtConfig = jwtConfig.Value;
			_context = context;
		}

		public async Task<TokensDto> GetTokens(User user, string ipAddress)
		{
			string jwt = GenerateJwt(user);
			string refreshTokenString = GenerateRefreshToken();

			var refreshToken = new UserRefreshToken
			{
				CreationDate = DateTime.Now,
				ExpirationDate = DateTime.Now.AddDays(_jwtConfig.RefreshTokenLifetimeInDays),
				IpAddress = ipAddress,
				IsInvalidated = false,
				RefreshToken = refreshTokenString,
				Token = jwt,
				UserId = user.Id
			};
			
			await _context.RefreshTokens.AddAsync(refreshToken);
			await _context.SaveChangesAsync();

			return new TokensDto
			{
				Jwt = jwt,
				RefreshToken = refreshTokenString
			};
		}

		public JwtSecurityToken GetJwt(string token)
		{
			JwtSecurityTokenHandler tokenHander = new JwtSecurityTokenHandler();
			return tokenHander.ReadJwtToken(token);
		}

		private string GenerateJwt(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);

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

		private string GenerateRefreshToken()
		{
			var byteArray = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(byteArray);
		}
	}
}
