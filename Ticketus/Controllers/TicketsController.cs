using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ticketus.Data;
using Ticketus.DTOs;
using Ticketus.Models;

namespace Ticketus.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly TicketDbContext _context;

        public TicketsController(TicketDbContext context)
        {
            _context = context;
        }

        // GET: api/Tickets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets([FromQuery] PaginationFilter filter)
        {
            var query = _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.User)
                .AsQueryable();

            // Apply filtering based on search term (description)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(t => t.Description.Contains(filter.SearchTerm));
            }

            // Get total count for pagination
            var totalTickets = await query.CountAsync();

            // Apply pagination
            var tickets = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    Description = t.Description,
                    Status = t.Status.StatusName,
                    DateCreated = t.DateCreated
                })
                .ToListAsync();

            // Return total count along with paginated data
            return Ok(new
            {
                TotalCount = totalTickets,
                PageSize = filter.PageSize,
                PageNumber = filter.PageNumber,
                Tickets = tickets
            });
        }

        // GET: api/Tickets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.User)
                .Select(t => new TicketDto
                {
                    TicketId = t.TicketId,
                    Description = t.Description,
                    Status = t.Status.StatusName,
                    DateCreated = t.DateCreated
                })
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        // POST: api/Tickets
        [HttpPost]
        public async Task<ActionResult<TicketDto>> CreateTicket(TicketCreateDto ticketCreateDto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.StatusName == ticketCreateDto.Status);
            if (status == null)
            {
                return BadRequest("Invalid Status");
            }

            var ticket = new Ticket
            {
                Description = ticketCreateDto.Description,
                StatusId = status.StatusId,
                UserId = userId,
                DateCreated = ticketCreateDto.DateCreated
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var ticketDto = new TicketDto
            {
                TicketId = ticket.TicketId,
                Description = ticket.Description,
                Status = status.StatusName,
                DateCreated = ticket.DateCreated
            };

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.TicketId }, ticketDto);
        }

        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, TicketUpdateDto updateTicketDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.StatusName == updateTicketDto.Status);
            if (status == null)
            {
                return BadRequest("Invalid Status");
            }

            ticket.Description = updateTicketDto.Description;
            ticket.StatusId = status.StatusId;
            ticket.UserId = userId;
            ticket.DateCreated = updateTicketDto.DateCreated;

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Tickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }
    }
}
