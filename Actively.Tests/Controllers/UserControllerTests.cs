using Actively.Controllers;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.PasswordHasher.Interfaces;
using Actively.Services.StatisticsCalculator.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace Actively.Tests.Controllers
{
    public class UserControllerTests
    {
		private readonly ITokenService _tokenService;
		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IPasswordHasher _passwordHasher;
        private readonly IActivityRepository _activityRepository;
        private readonly IStatisticsCalculator _statisticsCalculator;

        public UserControllerTests()
        {
            _tokenService=A.Fake<ITokenService>();
            _userRepository=A.Fake<IUserRepository>();
            _refreshTokenRepository=A.Fake<IRefreshTokenRepository>();
            _passwordHasher=A.Fake<IPasswordHasher>();
            _activityRepository=A.Fake<IActivityRepository>();
            _statisticsCalculator = A.Fake<IStatisticsCalculator>();
        }

		[Fact]
        public async Task RegisterUser_AlreadyUsedEmail_ReturnsBadRequestObjectResult()
        {
            //arrange
            var user = A.Dummy<User>();
            var registerUserDto = A.Dummy<RegisterUserDto>();
            var email = A.Dummy<string>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(email)).Returns(Task.FromResult(user));

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.RegisterUser(registerUserDto);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task RegisterUser_UnusedEmail_ReturnsOkObjectResult()
        {
            //arrange
            var registerUserDto = A.Dummy<RegisterUserDto>();
            User? returnValue = null;
            var user = A.Dummy<User>();
            var hashedPassword = A.Dummy<string>();
            var ipAddressString = A.Dummy<string>();
            var tokens = A.Dummy<TokensDto>();
            var ipAddress = A.Dummy<IPAddress>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(registerUserDto.Email)).Returns(Task.FromResult(returnValue));
            A.CallTo(() => _userRepository.RegisterUserAsync(registerUserDto, hashedPassword)).Returns(Task.FromResult(user));
            A.CallTo(() => _tokenService.GetTokens(user, ipAddressString)).Returns(tokens);

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            A.CallTo(() => controller.ControllerContext.HttpContext.Connection.RemoteIpAddress).Returns(ipAddress);

            //act
            var actionResult = await controller.RegisterUser(registerUserDto);

            //assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task RegisterUser_NullTokens_ReturnsBadRequestObjectResult()
        {
            //arrange
            var user = A.Dummy<User>();
            var registerUserDto = A.Dummy<RegisterUserDto>();
            var ipAddressString = A.Dummy<string>();
            TokensDto returnValue = null;

            A.CallTo(() => _tokenService.GetTokens(user, ipAddressString)).Returns(returnValue);

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.RegisterUser(registerUserDto);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_UnusedEmail_ReturnsNotFoundObjectResult()
        {
            //arrange
            User? returnValue = null;
            var loginUserDto = A.Dummy<LoginUserDto>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(loginUserDto.Email)).Returns(Task.FromResult(returnValue));

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(loginUserDto);

            //assert
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_WrongPassword_ReturnsUnauthorizedObjectResult()
        {
            //arrange
            var loginUserDto = A.Dummy<LoginUserDto>();
            var hashedPassword = A.Dummy<string>();

            A.CallTo(() => _passwordHasher.Verify(loginUserDto.Password, hashedPassword)).Returns(false);

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(loginUserDto);

            //assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_CorrectEmailAndPassword_ReturnsOkObjectResult()
        {
            //arrange
            var loginUserDto = A.Dummy<LoginUserDto>();
            var hashedPassword = A.Dummy<string>();
            var user = A.Dummy<User>();
            var tokens = A.Dummy<TokensDto>();
            var ipAddressString = A.Dummy<string>();
            var ipAddress = A.Dummy<IPAddress>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(loginUserDto.Email)).Returns(Task.FromResult(user));
            A.CallTo(() => _passwordHasher.Verify(hashedPassword, loginUserDto.Password)).Returns(true);
            A.CallTo(() => _tokenService.GetTokens(user, ipAddressString)).Returns(tokens);

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            A.CallTo(() => controller.ControllerContext.HttpContext.Connection.RemoteIpAddress).Returns(ipAddress);

            //act
            var actionResult = await controller.LoginUser(loginUserDto);

            //assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_NullTokens_ReturnsUnauthorizedObjectResult()
        {
            //arrange
            var loginUserDto = A.Dummy<LoginUserDto>();
            var hashedPassword = A.Dummy<string>();
            var user = A.Dummy<User>();
            var ipAddressString = A.Dummy<string>();
            TokensDto returnValue = null;

            A.CallTo(() => _tokenService.GetTokens(user, ipAddressString)).Returns(returnValue);

            var controller = new UserController(_tokenService, _userRepository, _activityRepository, _refreshTokenRepository,
                _passwordHasher, _statisticsCalculator);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(loginUserDto);

            //assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }
    }
}