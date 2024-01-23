using MyFitBook.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyFitBook.Context;
using MyFitBook.Models;
using MyFitBook.Models.DTOs;
using MyFitBook.Services.AuthService.Configuration;
using MyFitBook.Services.AuthService.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyFitBook.Services.AuthService
{
	/// <summary>
	/// Class that provides basic operations on JWTs
	/// </summary>
	public class TokenService : ITokenService
	{
		private readonly JwtConfig _jwtConfig;
		private readonly MyFitBookDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenService"/> class.
		/// </summary>
		/// <param name="jwtConfig"></param>
		/// <param name="context"></param>
		public TokenService(IOptions<JwtConfig> jwtConfig, MyFitBookDbContext context)
		{
			_jwtConfig = jwtConfig.Value;
			_context = context;
		}

		/// <summary>
		/// Returns provided user's access and refresh tokens
		/// </summary>
		/// <param name="user"></param>
		/// <param name="ipAddress"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Converts <c>string</c> to <c>JwtSecurityTokenHandler</c>
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public JwtSecurityToken GetJwt(string token)
		{
			JwtSecurityTokenHandler tokenHander = new JwtSecurityTokenHandler();
			return tokenHander.ReadJwtToken(token);
		}

		/// <summary>
		/// Generates new JWT for provided user
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Generates new refresh token
		/// </summary>
		/// <returns></returns>
		private string GenerateRefreshToken()
		{
			var byteArray = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(byteArray);
		}
	}
}
