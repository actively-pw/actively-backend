using Actively.Controllers;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.PasswordHasher.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Tests
{
	public class UserControllerTests
	{
		[Fact]
		public async Task RegisterUser_AlreadyUsedEmail_ReturnsBadRequestObjectResult()
		{
			//arrange
			var tokenService = A.Fake<ITokenService>();
			var userRepository = A.Fake<IUserRepository>();
			var passwordHasher = A.Fake<IPasswordHasher>();

			var fakeUser = A.Dummy<User>();
			var fakeRegisterUserDto = A.Dummy<RegisterUserDto>();
			var fakeEmail = A.Dummy<string>();

			A.CallTo(() => userRepository.GetUserByEmailAsync(fakeEmail)).Returns(Task.FromResult(fakeUser));

			var controller = new UserController(tokenService, userRepository, passwordHasher);

			//act
			var actionResult = await controller.RegisterUser(fakeRegisterUserDto);

			//assert
			Assert.IsType<BadRequestObjectResult>(actionResult);
		}

		[Fact]
		public async Task RegisterUser_UnusedEmail_ReturnsOkObjectResult()
		{
			//arrange
			var tokenService = A.Fake<ITokenService>();
			var userRepository = A.Fake<IUserRepository>();
			var passwordHasher = A.Fake<IPasswordHasher>();

			var fakeRegisterUserDto = A.Dummy<RegisterUserDto>();
			User? returnValue = null;
			var fakeUser = A.Dummy<User>();
			var fakeHashedPassword = A.Dummy<string>();

			A.CallTo(() => userRepository.GetUserByEmailAsync(fakeRegisterUserDto.Email)).Returns(Task.FromResult(returnValue));
			A.CallTo(() => userRepository.RegisterUserAsync(fakeRegisterUserDto, fakeHashedPassword)).Returns(Task.FromResult(fakeUser));

			var controller = new UserController(tokenService, userRepository, passwordHasher);

			//act
			var actionResult = await controller.RegisterUser(fakeRegisterUserDto);

			//assert
			Assert.IsType<OkObjectResult>(actionResult);
		}

		[Fact]
		public async Task LoginUser_UnusedEmail_ReturnsNotFoundObjectResult()
		{
			//arrange
			var tokenService = A.Fake<ITokenService>();
			var userRepository = A.Fake<IUserRepository>();
			var passwordHasher = A.Fake<IPasswordHasher>();

			User? returnValue = null;
			var fakeLoginUserDto = A.Dummy<LoginUserDto>();

			A.CallTo(() => userRepository.GetUserByEmailAsync(fakeLoginUserDto.Email)).Returns(Task.FromResult(returnValue));

			var controller = new UserController(tokenService, userRepository, passwordHasher);

			//act
			var actionResult = await controller.LoginUser(fakeLoginUserDto);

			//assert
			Assert.IsType<NotFoundObjectResult>(actionResult);
		}

		[Fact]
		public async Task LoginUser_WrongPassword_ReturnsUnauthorizedObjectResult()
		{
			//arrange
			var tokenService = A.Fake<ITokenService>();
			var userRepository = A.Fake<IUserRepository>();
			var passwordHasher = A.Fake<IPasswordHasher>();

			var fakeLoginUserDto = A.Dummy<LoginUserDto>();
			var fakeHashedPassword = A.Dummy<string>();

			A.CallTo(() => passwordHasher.Verify(fakeLoginUserDto.Password, fakeHashedPassword)).Returns(false);

			var controller = new UserController(tokenService, userRepository, passwordHasher);

			//act
			var actionResult = await controller.LoginUser(fakeLoginUserDto);

			//assert
			Assert.IsType<UnauthorizedObjectResult>(actionResult);
		}

		[Fact]
		public async Task LoginUser_CorrectEmailAndPassword_ReturnsOkObjectResult()
		{
			//arrange
			var tokenService = A.Fake<ITokenService>();
			var userRepository = A.Fake<IUserRepository>();
			var passwordHasher = A.Fake<IPasswordHasher>();

			var fakeLoginUserDto = A.Dummy<LoginUserDto>();
			var fakeHashedPassword = A.Dummy<string>();
			var fakeUser = A.Dummy<User>();

			A.CallTo(() => userRepository.GetUserByEmailAsync(fakeLoginUserDto.Email)).Returns(Task.FromResult(fakeUser));
			A.CallTo(() => passwordHasher.Verify(fakeHashedPassword, fakeLoginUserDto.Password)).Returns(true);

			var controller = new UserController(tokenService, userRepository, passwordHasher);

			//act
			var actionResult = await controller.LoginUser(fakeLoginUserDto);

			//assert
			Assert.IsType<OkObjectResult>(actionResult);
		}
	}
}