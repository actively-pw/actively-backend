using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFitBook.Controllers.Repositories.Interfaces;
using MyFitBook.Models.DTOs;
using MyFitBook.Models.DTOs.Statistics;
using MyFitBook.Models.Enums;
using MyFitBook.Services.AuthService.Interfaces;
using MyFitBook.Services.PasswordHasher.Interfaces;
using MyFitBook.Services.StatisticsCalculator.Interfaces;
using System.Net.Mime;

namespace MyFitBook.Controllers
{

	[Route("Users")]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[ApiController]
	public class UserController : Controller
	{
		private readonly ITokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly IActivityRepository _activityRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IStatisticsCalculator _statisticsCalculator;
		private const int _daysInWeek = 7;
		private const int _daysInYear = 365;
		public UserController(ITokenService tokenService, IUserRepository userRepository, IActivityRepository activityRepository,
			IRefreshTokenRepository refreshTokenRepository, IPasswordHasher passwordHasher, IStatisticsCalculator statisticsCalculator)
		{
			_tokenService = tokenService;
			_userRepository = userRepository;
			_activityRepository = activityRepository;
			_refreshTokenRepository = refreshTokenRepository;
			_passwordHasher = passwordHasher;
			_statisticsCalculator = statisticsCalculator;
		}

		/// <summary>
		/// Registers new user based on informartion provided in registerUserDto
		/// </summary>
		/// <param name="registerUserDto"></param>
		/// <returns>Action result</returns>
		[HttpPost("register")]
		[ProducesResponseType(StatusCodes.Status200OK)] // czy 201?
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
		{
			if (await _userRepository.GetUserByEmailAsync(registerUserDto.Email) is not null)
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

		/// <summary>
		/// Allows user to log in, unless provided password is incorrect for given e-mail
		/// </summary>
		/// <param name="loginUserDto"></param>
		/// <returns>Access and refresh tokens (if login was successful)</returns>
		[HttpPost("login")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> LoginUser([FromBody] LoginUserDto loginUserDto)
		{
			var user = await _userRepository.GetUserByEmailAsync(loginUserDto.Email);

			if (user is null)
			{
				return NotFound(
					new
					{
						message = "User not found"
					});
			}

			if (!_passwordHasher.Verify(user.Password, loginUserDto.Password))
			{
				return Unauthorized(
					new
					{
						message = "Incorrect email or password"
					});
			}

			var tokens = _tokenService.GetTokens(user, HttpContext.Connection.RemoteIpAddress.ToString());

			if (tokens is null)
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

		/// <summary>
		/// Refreshes user's tokens
		/// </summary>
		/// <remarks>
		/// Firstly, the refresh token is invalidated. Then new JWT access token and new refresh token are generated.
		/// </remarks>
		/// <param name="tokensDto"></param>
		/// <returns>New access and refresh tokens (or error message)</returns>
		[HttpPost("refreshToken")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult> RefreshToken([FromBody] TokensDto tokensDto)
		{
			if (!ModelState.IsValid)
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

		/// <summary>
		/// Returns information about user whose tokens are provided
		/// </summary>
		/// <returns>User information - name, surname and e-mail address</returns>
		[HttpGet("me")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<UserInfoDto>> GetUserInformation()
		{
			try
			{
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				var jwt = _tokenService.GetJwt(accessToken);
				var userId = jwt.Claims.FirstOrDefault().Value;
				var userIdGuid = new Guid(userId);

				var user = await _userRepository.GetUserByIdAsync(userIdGuid);
				if (user is null) return NotFound($"User with id {userId} does not exist");

				return Ok(new UserInfoDto(user));
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get user information: {ex.Message}");
			}
		}

		/// <summary>
		/// Returns user's summary statistics
		/// </summary>
		/// <remarks>
		/// The summary statistics consist of last 7 days, last 365 days and all time statistics. Each category is grouped by sport discipline.
		/// </remarks>
		/// <returns>Summary statistics - weekly, yearly and all time</returns>
		[HttpGet("summaryStatistics")]
		[Authorize]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<SummaryStatisticsDto>> GetSummaryStatistics()
		{
			try
			{
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				var jwt = _tokenService.GetJwt(accessToken);
				var userId = jwt.Claims.FirstOrDefault().Value;
				var userIdGuid = new Guid(userId);

				Dictionary<Sport, SportSummaryDto> sportSummaries = new();

				foreach (Sport sport in Enum.GetValues(typeof(Sport)))
				{
					var lastWeekActivities = await _activityRepository.GetLatestActivitiesByDaysCountAndSport(_daysInWeek, sport, userIdGuid); // todo: not found
					var lastYearActivities = await _activityRepository.GetLatestActivitiesByDaysCountAndSport(_daysInYear, sport, userIdGuid);
					var allTimeActivities = await _activityRepository.GetActivitiesBySport(sport, userIdGuid);

					var statistics = _statisticsCalculator.CalculateSportSummary(lastWeekActivities, lastYearActivities, allTimeActivities);
					sportSummaries.Add(sport, new SportSummaryDto(sport, statistics));
				}

				return Ok(new SummaryStatisticsDto
				{
					Cycling = sportSummaries[Sport.BicycleRide],
					Running = sportSummaries[Sport.Run],
					NordicWalking = sportSummaries[Sport.NordicWalking]
				});
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get summary statistics: {ex.Message}");
			}
		}

	}
}
