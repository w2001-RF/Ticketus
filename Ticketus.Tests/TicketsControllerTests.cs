using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ticketus.Controllers;
using Ticketus.Data;
using Ticketus.DTOs;
using Ticketus.Models;
using Xunit;

namespace Ticketus.Tests
{
    public class TicketsControllerTests
    {
        private readonly TicketDbContext _context;
        private readonly TicketsController _controller;
        private readonly Mock<ClaimsPrincipal> _mockUser;

        public TicketsControllerTests()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<TicketDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TicketDbContext(options);

            // Set up a mock user
            _mockUser = new Mock<ClaimsPrincipal>();
            _mockUser.Setup(u => u.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "1"));

            // Initialize the controller with the in-memory context
            _controller = new TicketsController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = _mockUser.Object
                    }
                }
            };
        }

        [Fact]
        public async Task GetTickets_ReturnsOkResult_WithTickets()
        {
            // Arrange
            var ticket = new Ticket { TicketId = 1, Description = "Test Ticket", UserId = 1, StatusId = 1, DateCreated = DateTime.Now };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var filter = new PaginationFilter { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _controller.GetTickets(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tickets = Assert.IsAssignableFrom<IEnumerable<TicketDto>>(okResult.Value);
            Assert.Single(tickets);
        }

        [Fact]
        public async Task CreateTicket_ReturnsCreatedAtAction_WithTicketDto()
        {
            // Arrange
            var status = new Status { StatusId = 1, StatusName = "Open" };
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();

            var ticketCreateDto = new TicketCreateDto
            {
                Description = "New Test Ticket",
                Status = status.StatusName,
                DateCreated = DateTime.Now
            };

            // Act
            var result = await _controller.CreateTicket(ticketCreateDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var ticketDto = Assert.IsType<TicketDto>(createdResult.Value);
            Assert.Equal("New Test Ticket", ticketDto.Description);
        }

        [Fact]
        public async Task UpdateTicket_ReturnsNoContent_WhenTicketExists()
        {
            // Arrange
            var ticket = new Ticket { TicketId = 1, Description = "Old Ticket", UserId = 1, StatusId = 1, DateCreated = DateTime.Now };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var ticketUpdateDto = new TicketUpdateDto
            {
                Description = "Updated Ticket",
                Status = "Open",
                DateCreated = DateTime.Now
            };

            // Act
            var result = await _controller.UpdateTicket(ticket.TicketId, ticketUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedTicket = await _context.Tickets.FindAsync(ticket.TicketId);
            Assert.Equal("Updated Ticket", updatedTicket.Description);
        }

        [Fact]
        public async Task DeleteTicket_ReturnsNoContent_WhenTicketExists()
        {
            // Arrange
            var ticket = new Ticket { TicketId = 1, Description = "Delete Me", UserId = 1, StatusId = 1, DateCreated = DateTime.Now };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteTicket(ticket.TicketId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deletedTicket = await _context.Tickets.FindAsync(ticket.TicketId);
            Assert.Null(deletedTicket);
        }
    }
}
