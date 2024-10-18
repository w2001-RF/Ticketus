    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Ticketus.Data;
    using Ticketus.DTOs;
    using Ticketus.Models;
    using Ticketus.Services;

    namespace Ticketus.Controllers
    {
        [Route("api/v1/[controller]")]
        [ApiController]
        public class AuthenticationController : ControllerBase
        {
            private readonly TicketDbContext _context;
            private readonly JwtService _jwtService;

            public AuthenticationController(TicketDbContext context, JwtService jwtService)
            {
                _context = context;
                _jwtService = jwtService;
            }

            // POST: api/Authentication/signup
            [HttpPost("signup")]
            public async Task<ActionResult<User>> Signup(SignupDto signupDto)
            {
                if (await _context.Users.AnyAsync(u => u.Email == signupDto.Email))
                {
                    return BadRequest("Email already in use");
                }

                CreatePasswordHash(signupDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = new User
                {
                    UserName = signupDto.UserName,
                    Email = signupDto.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    TicketCount = user.Tickets.Count
                };

                return Ok(userDto);
            }

            // POST: api/Authentication/login
            [HttpPost("login")]
            public async Task<ActionResult<string>> Login(LoginDto loginDto)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = _jwtService.GenerateJwtToken(user);
                return Ok(new
                { 
                    token= token
                });
            }

            // Utility function to create a password hash and salt
            private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
            {
                using (var hmac = new HMACSHA512())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
            }

            // Utility function to verify password hash
            private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
            {
                using (var hmac = new HMACSHA512(storedSalt))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i]) return false;
                    }
                }
                return true;
            }
        }
    }
