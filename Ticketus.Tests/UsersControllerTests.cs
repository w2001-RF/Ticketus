using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ticketus.Controllers;
using Ticketus.Data;
using Ticketus.DTOs;
using Ticketus.Models;
using Xunit;

namespace Ticketus.Tests
{
    public class UsersControllerTests
    {
        private readonly TicketDbContext _context;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TicketDbContext(options);

            // Initialize the controller with the in-memory context
            _controller = new UsersController(_context);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithUserDtos()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "testuser", Email = "test@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Single(users);
        }

        [Fact]
        public async Task GetUser_ReturnsOkResult_WithUserDto()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "testuser", Email = "test@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetUser(user.UserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(user.UserId, userDto.UserId);
        }

        [Fact]
        public async Task CreateUser_ReturnsCreatedAtActionResult_WithUserDto()
        {
            // Arrange
            var userCreateDto = new UserCreateDto
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123"
            };

            // Act
            var result = await _controller.CreateUser(userCreateDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var userDto = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal(userCreateDto.UserName, userDto.UserName);
            Assert.Equal(userCreateDto.Email, userDto.Email);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "olduser", Email = "old@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userUpdateDto = new UserUpdateDto
            {
                UserName = "updateduser",
                Email = "updated@example.com"
            };

            // Act
            var result = await _controller.UpdateUser(user.UserId, userUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedUser = await _context.Users.FindAsync(user.UserId);
            Assert.Equal("updateduser", updatedUser.UserName);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
        {
            // Arrange
            var user = new User { UserId = 1, UserName = "deleteuser", Email = "delete@example.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteUser(user.UserId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedUser = await _context.Users.FindAsync(user.UserId);
            Assert.Null(deletedUser);
        }
    }
}