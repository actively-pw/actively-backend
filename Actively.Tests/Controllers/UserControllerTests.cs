using Actively.Controllers;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.PasswordHasher.Interfaces;
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

        public UserControllerTests()
        {
            _tokenService=A.Fake<ITokenService>();
            _userRepository=A.Fake<IUserRepository>();
            _refreshTokenRepository=A.Fake<IRefreshTokenRepository>();
            _passwordHasher=A.Fake<IPasswordHasher>();
        }

		[Fact]
        public async Task RegisterUser_AlreadyUsedEmail_ReturnsBadRequestObjectResult()
        {
            //arrange
            var fakeUser = A.Dummy<User>();
            var fakeRegisterUserDto = A.Dummy<RegisterUserDto>();
            var fakeEmail = A.Dummy<string>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(fakeEmail)).Returns(Task.FromResult(fakeUser));

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.RegisterUser(fakeRegisterUserDto);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task RegisterUser_UnusedEmail_ReturnsOkObjectResult()
        {
            //arrange
            var fakeRegisterUserDto = A.Dummy<RegisterUserDto>();
            User? returnValue = null;
            var fakeUser = A.Dummy<User>();
            var fakeHashedPassword = A.Dummy<string>();
            var fakeIpAddressString = A.Dummy<string>();
            var fakeTokens = A.Dummy<TokensDto>();
            var fakeIpAddress = A.Dummy<IPAddress>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(fakeRegisterUserDto.Email)).Returns(Task.FromResult(returnValue));
            A.CallTo(() => _userRepository.RegisterUserAsync(fakeRegisterUserDto, fakeHashedPassword)).Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _tokenService.GetTokens(fakeUser, fakeIpAddressString)).Returns(fakeTokens);

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            A.CallTo(() => controller.ControllerContext.HttpContext.Connection.RemoteIpAddress).Returns(fakeIpAddress);

            //act
            var actionResult = await controller.RegisterUser(fakeRegisterUserDto);

            //assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task RegisterUser_NullTokens_ReturnsBadRequestObjectResult()
        {
            //arrange
            var fakeUser = A.Dummy<User>();
            var fakeRegisterUserDto = A.Dummy<RegisterUserDto>();
            var fakeIpAddressString = A.Dummy<string>();
            TokensDto returnValue = null;

            A.CallTo(() => _tokenService.GetTokens(fakeUser, fakeIpAddressString)).Returns(returnValue);

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.RegisterUser(fakeRegisterUserDto);

            //assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_UnusedEmail_ReturnsNotFoundObjectResult()
        {
            //arrange
            User? returnValue = null;
            var fakeLoginUserDto = A.Dummy<LoginUserDto>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(fakeLoginUserDto.Email)).Returns(Task.FromResult(returnValue));

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(fakeLoginUserDto);

            //assert
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_WrongPassword_ReturnsUnauthorizedObjectResult()
        {
            //arrange
            var fakeLoginUserDto = A.Dummy<LoginUserDto>();
            var fakeHashedPassword = A.Dummy<string>();

            A.CallTo(() => _passwordHasher.Verify(fakeLoginUserDto.Password, fakeHashedPassword)).Returns(false);

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(fakeLoginUserDto);

            //assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_CorrectEmailAndPassword_ReturnsOkObjectResult()
        {
            //arrange
            var fakeLoginUserDto = A.Dummy<LoginUserDto>();
            var fakeHashedPassword = A.Dummy<string>();
            var fakeUser = A.Dummy<User>();
            var fakeTokens = A.Dummy<TokensDto>();
            var fakeIpAddressString = A.Dummy<string>();
            var fakeIpAddress = A.Dummy<IPAddress>();

            A.CallTo(() => _userRepository.GetUserByEmailAsync(fakeLoginUserDto.Email)).Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _passwordHasher.Verify(fakeHashedPassword, fakeLoginUserDto.Password)).Returns(true);
            A.CallTo(() => _tokenService.GetTokens(fakeUser, fakeIpAddressString)).Returns(fakeTokens);

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            A.CallTo(() => controller.ControllerContext.HttpContext.Connection.RemoteIpAddress).Returns(fakeIpAddress);

            //act
            var actionResult = await controller.LoginUser(fakeLoginUserDto);

            //assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async Task LoginUser_NullTokens_ReturnsUnauthorizedObjectResult()
        {
            //arrange
            var fakeLoginUserDto = A.Dummy<LoginUserDto>();
            var fakeHashedPassword = A.Dummy<string>();
            var fakeUser = A.Dummy<User>();
            var fakeIpAddressString = A.Dummy<string>();
            TokensDto returnValue = null;

            A.CallTo(() => _tokenService.GetTokens(fakeUser, fakeIpAddressString)).Returns(returnValue);

            var controller = new UserController(_tokenService, _userRepository, _refreshTokenRepository, _passwordHasher);

            controller.ControllerContext = A.Dummy<ControllerContext>();
            controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

            //act
            var actionResult = await controller.LoginUser(fakeLoginUserDto);

            //assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult);
        }
    }
}