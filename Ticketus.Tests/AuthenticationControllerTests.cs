namespace Ticketus.Tests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Ticketus.Controllers;
using Ticketus.Data;
using Ticketus.DTOs;
using Ticketus.Models;
using Ticketus.Services;
using Xunit;

namespace Ticketus.Tests
{
    public class AuthenticationControllerTests
    {
        private readonly TicketDbContext _context;
        private readonly Mock<JwtService> _jwtServiceMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            // In-memory database for testing
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TicketDbContext(options);
            _jwtServiceMock = new Mock<JwtService>(null);
            _controller = new AuthenticationController(_context, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Signup_ReturnsBadRequest_WhenEmailExists()
        {
            // Arrange
            var signupDto = new SignupDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            // Add a user to the in-memory database
            _context.Users.Add(new User { Email = signupDto.Email });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email already in use", actionResult.Value);
        }

        [Fact]
        public async Task Signup_CreatesUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var signupDto = new SignupDto
            {
                UserName = "newuser",
                Email = "new@example.com",
                Password = "password123"
            };

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(actionResult.Value);
            Assert.Equal(signupDto.Email, user.Email);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid credentials", actionResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsToken_WhenValidCredentials()
        {
            // Arrange
            var signupDto = new SignupDto
            {
                UserName = "validuser",
                Email = "valid@example.com",
                Password = "password123"
            };

            await _controller.Signup(signupDto);

            var loginDto = new LoginDto
            {
                Email = signupDto.Email,
                Password = signupDto.Password
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var tokenResult = Assert.IsType<string>(actionResult.Value);
            Assert.NotNull(tokenResult);
        }
    }
}
