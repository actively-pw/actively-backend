using MyFitBook.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyFitBook.BlobStorage.Interfaces;
using MyFitBook.Controllers.Repositories.Interfaces;
using MyFitBook.Models;
using MyFitBook.Models.DTOs;
using MyFitBook.Services.AuthService.Interfaces;
using MyFitBook.Services.GeoJsonGenerator.Interfaces;
using MyFitBook.Services.StaticMapGenerator.Interfaces;
using MyFitBook.Services.StatisticsCalculator.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyFitBook.Tests.Controllers
{
	public class ActivityControllerTests
	{
		private readonly IActivityRepository _activityRepository;
		private readonly IStorageManager _blobStorage;
		private readonly IGeoJsonGenerator _geoJsonGenerator;
		private readonly IStaticMapGenerator _staticMapGenerator;
		private readonly IStatisticsCalculator _statisticsCalculator;
		private readonly ITokenService _tokenService;

		public ActivityControllerTests()
		{
			_activityRepository = A.Fake<IActivityRepository>();
			_blobStorage = A.Fake<IStorageManager>();
			_geoJsonGenerator = A.Fake<IGeoJsonGenerator>();
			_staticMapGenerator = A.Fake<IStaticMapGenerator>();
			_statisticsCalculator = A.Fake<IStatisticsCalculator>();
			_tokenService = A.Fake<ITokenService>();
		}
		[Fact]
		public async Task GetAllActivities_ThereIsAtLeastOneActivity_ReturnsGetActivityDtoList()
		{
			//arrange
			PaginationParams @params = A.Dummy<PaginationParams>();
			List<Activity> activitiesList = new List<Activity> { new Activity() };
			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			string staticMapType = "web";

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			A.CallTo(() => _activityRepository.GetActivitiesByUserId(user.Id)).Returns(Task.FromResult(activitiesList));

			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.GetActivitiesByUserId(staticMapType, @params);

			//assert
			var objectResult = (ObjectResult)result.Result;
			var enumerable = objectResult.Value as IEnumerable<GetActivityDto>;

			Assert.NotNull(objectResult);
			Assert.IsType<ActionResult<List<GetActivityDto>>>(result);
		}

		[Theory]
		[InlineData(1, 3, 5)]
		[InlineData(2, 3, 5)]
		[InlineData(2, 3, 10)]
		[InlineData(4, 3, 10)]
		[InlineData(5, 3, 10)]
		public async Task GetAllActivities_NaturalNumberPaginationParams_CanPaginate(int page, int itemsPerPage, int itemsCount)
		{
			//arrange
			PaginationParams @params = new PaginationParams()
			{
				Page = page,
				ItemsPerPage = itemsPerPage
			};

			List<Activity> activitiesList = new List<Activity>();
			for (int i = 0; i < itemsCount; i++)
			{
				activitiesList.Add(new Activity());
				activitiesList[i].Title = i.ToString();
			}

			string staticMapType = "web";

			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			A.CallTo(() => _tokenService.GetJwt("accessTokenValue")).Returns(jwt);

			A.CallTo(() => _activityRepository.GetActivitiesByUserId(user.Id)).Returns(Task.FromResult(activitiesList));

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.GetActivitiesByUserId(staticMapType, @params);

			//assert
			var objectResult = (ObjectResult)result.Result;
			var enumerable = objectResult.Value as IEnumerable<GetActivityDto>;

			if (page * itemsPerPage < itemsCount)
			{
				Assert.Equal(enumerable.Count(), itemsPerPage);
			}
			else
			{
				Assert.Equal(enumerable.Count(), itemsCount - (page - 1) * itemsPerPage > 0 ? itemsCount - (page - 1) * itemsPerPage : 0);
			}
		}

		[Fact]
		public async Task GetAllActivities_NoActivitiesInDb_ReturnsOkObjectResultAndNullValue()
		{
			//arrange
			PaginationParams @params = A.Dummy<PaginationParams>();

			List<Activity> emptyList = new List<Activity>();

			string staticMapType = "web";

			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			A.CallTo(() => _activityRepository.GetActivitiesByUserId(user.Id)).Returns(Task.FromResult(emptyList));
			A.CallTo(() => _tokenService.GetJwt("accessTokenValue")).Returns(jwt);

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.GetActivitiesByUserId(staticMapType, @params);

			//assert
			Assert.IsType<OkObjectResult>(result.Result);
			Assert.Null(result.Value);
		}

		[Theory]
		[InlineData(1, 3, 5, 2)]
		[InlineData(2, 3, 5, -1)]
		public async Task GetAllActivities_CanCalculateNextPage(int page, int itemsPerPage, int itemsCount, int correctNextPage)
		{
			//arrange
			PaginationParams @params = new PaginationParams()
			{
				Page = page,
				ItemsPerPage = itemsPerPage
			};

			string staticMapType = "web";

			List<Activity> activitiesList = new List<Activity>();
			for (int i = 0; i < itemsCount; i++)
			{
				activitiesList.Add(new Activity());
			}

			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			HttpResponse response = A.Fake<HttpResponse>();

			A.CallTo(() => _activityRepository.GetActivitiesByUserId(user.Id)).Returns(Task.FromResult(activitiesList));
			A.CallTo(() => _activityRepository.GetActivitiesCount()).Returns(itemsCount);
			A.CallTo(() => _tokenService.GetJwt("accessTokenValue")).Returns(jwt);

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.GetActivitiesByUserId(staticMapType, @params);

			//assert
			Assert.True(controller.Response.Headers.ContainsKey("nextPage"));
			int.TryParse(controller.Response.Headers["nextPage"], out int calculatedNextPage);
			Assert.Equal(correctNextPage, calculatedNextPage);
		}

		[Fact]
		public async Task AddActivity_ActivityWithGivenIdAlreadyExists_ReturnsBadRequest()
		{
			//arrange
			var addActivityDto = A.Dummy<AddActivityDto>();
			var activity = A.Dummy<Activity>();

			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			A.CallTo(() => _tokenService.GetJwt("accessTokenValue")).Returns(jwt);

			A.CallTo(() => _activityRepository.GetActivityById(addActivityDto.Id)).Returns(activity);

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);


			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.AddActivity(addActivityDto);

			//assert
			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task AddActivity_ValidAddActivityDto_ReturnsOkObjectResult()
		{
			//arrange
			var addActivityDto = new AddActivityDto();
			Activity? returnValue = null;
			User user = A.Fake<User>();

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
	{
					new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email)
				})
			};

			JwtSecurityToken jwt = tokenHandler.ReadJwtToken(tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor)));

			var authToken = new AuthenticationToken { Name = "access_token", Value = "accessTokenValue" };

			A.CallTo(() => _activityRepository.GetActivityById(addActivityDto.Id)).Returns(returnValue);
			A.CallTo(() => _tokenService.GetJwt("accessTokenValue")).Returns(jwt);


			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			controller.ControllerContext = new ControllerContext();
			var serviceProvider = A.Fake<IServiceProvider>();
			var authService = A.Fake<IAuthenticationService>();
			var authResult = AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(), string.Empty));

			authResult.Properties!.StoreTokens(new[]
				{
					authToken
				}
			);

			controller.ControllerContext.HttpContext = new DefaultHttpContext
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim("userid", user.Id.ToString()),
					new Claim("email", user.Email) })),
				RequestServices = serviceProvider
			};

			A.CallTo(() => authService.AuthenticateAsync(controller.ControllerContext.HttpContext, null)).Returns(authResult);

			A.CallTo(() => serviceProvider.GetService(typeof(IAuthenticationService))).Returns(authService);

			//act
			var result = await controller.AddActivity(addActivityDto);

			//assert
			Assert.IsType<OkResult>(result);
		}

		[Fact]
		public async Task DeleteActivity_ValidId_ReturnsActionResultAndActivity()
		{
			//arrange
			var id = A.Dummy<Guid>();
			Activity activity = new Activity();
			activity.Id = id;

			A.CallTo(() => _activityRepository.DeleteActivity(id)).Returns(activity);

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			//act
			var result = await controller.DeleteActivity(id);

			//assert
			Assert.IsType<ActionResult<ActivityResponseDto>>(result);
		}

		[Fact]
		public async Task EditActivity_ValidId_ReturnsOkObjectResult()
		{
			//arrange
			var id = A.Dummy<Guid>();
			Activity activity = new Activity();
			activity.Id = id;
			JsonPatchDocument<Activity> patchDoc = A.Dummy<JsonPatchDocument<Activity>>();

			A.CallTo(() => _activityRepository.EditActivity(id, patchDoc)).Returns(activity);

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator,
				_staticMapGenerator, _statisticsCalculator, _tokenService);

			//act
			var result = await controller.EditActivity(id, patchDoc);

			//assert
			Assert.IsType<OkObjectResult>(result.Result);
		}
	}
}
