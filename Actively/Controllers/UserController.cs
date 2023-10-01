using Actively.Controllers.Repositories.Interfaces;
using Actively.Models.DTOs;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.PasswordHasher.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Actively.Controllers
{

	[Route("Users")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly ITokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IPasswordHasher _passwordHasher;
		public UserController(ITokenService tokenService, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IPasswordHasher passwordHasher)
		{
			_tokenService = tokenService;
			_userRepository = userRepository;
			_refreshTokenRepository = refreshTokenRepository;
			_passwordHasher = passwordHasher;
		}

		[HttpPost("register")]
		public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
		{
			if(await _userRepository.GetUserByEmailAsync(registerUserDto.Email) is not null)
			{
				return BadRequest("User with email " + registerUserDto.Email + " already exists.");
			}

			var passwordHash = _passwordHasher.Hash(registerUserDto.Password);

			var user = await _userRepository.RegisterUserAsync(registerUserDto, passwordHash);

			var tokens = _tokenService.GetTokens(user, HttpContext.Connection.RemoteIpAddress.ToString());

			if (tokens is null)
			{
				return Unauthorized(
					new
					{
						message = "Failed to generate tokens"
					});
			}

			return Ok(new
			{
				message = "User registered successfully",
				jwt = tokens.Result.Jwt,
				refreshToken = tokens.Result.RefreshToken
			});
		}

		[HttpPost("login")]
		public async Task<ActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
		{
			var user = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

			if(user is null)
			{
				return NotFound(
					new
					{
						message = "User not found"
					});
			}

			if(!_passwordHasher.Verify(user.Password, loginUserDto.Password))
			{
				return Unauthorized(
					new
					{
						message = "Incorrect email or password"
					});
			}

			var tokens = _tokenService.GetTokens(user, HttpContext.Connection.RemoteIpAddress.ToString());

			if(tokens is null)
			{
				return Unauthorized(
					new
					{
						message = "Failed to generate tokens"
					});
			}

			return Ok(
				new
				{
					message = "Successful login",
					jwt = tokens.Result.Jwt,
					refreshToken = tokens.Result.RefreshToken
				});
			;
		}

		[HttpPost("refreshToken")]
		public async Task<ActionResult> RefreshToken([FromBody] TokensDto tokensDto)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(
					new
					{
						message = "Tokens must be provided"
					});
			}

			var jwt = _tokenService.GetJwt(tokensDto.Jwt);

			var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

			try
			{
				var oldRefreshToken = await _refreshTokenRepository.InvalidateRefreshTokenAsync(ipAddress, jwt, tokensDto);
				var newTokens = _tokenService.GetTokens(oldRefreshToken.User, ipAddress);

				return Ok(
					new
					{
						message = "Successfully refreshed tokens",
						jwt = newTokens.Result.Jwt,
						refreshToken = newTokens.Result.RefreshToken
					});
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to refresh token: {ex.Message}");
			}
		}

	}
}
