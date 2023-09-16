using Actively.Controllers.Repositories.Interfaces;
using Actively.Models.DTOs;
using Actively.Services.AuthService;
using Actively.Services.PasswordHasher;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers
{

	[Route("Users")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly TokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly PasswordHasher _passwordHasher;
		public UserController(TokenService tokenService, IUserRepository userRepository, PasswordHasher passwordHasher)
		{
			_tokenService = tokenService;
			_userRepository = userRepository;
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

			var token = _tokenService.GenerateToken(user);

			return Ok(new
			{
				message = "User registered successfully",
				token
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

			var jwt = _tokenService.GenerateToken(user);

			return Ok(
				new
				{
					message = "Successful login",
					token = jwt
				});
		}

	}
}
