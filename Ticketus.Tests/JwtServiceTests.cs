using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;  // Added for Dictionary
using System.IdentityModel.Tokens.Jwt;
using System.Linq;  // Added for LINQ
using System.Text;
using Ticketus.Models;
using Ticketus.Services;
using Xunit;

namespace Ticketus.Tests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public JwtServiceTests()
        {
            // Mock configuration
            var config = new Dictionary<string, string>
            {
                {"Jwt:Key", "this_is_a_super_secret_key"},
                {"Jwt:Issuer", "test_issuer"},
                {"Jwt:Audience", "test_audience"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            _jwtService = new JwtService(_configuration);
        }

        [Fact]
        public void GenerateJwtToken_ReturnsValidToken()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@example.com"
            };

            // Act
            var token = _jwtService.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            // Validate the token
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.Equal(user.UserId.ToString(), jwtToken.Subject);
            Assert.Equal(user.Email, jwtToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(user.UserName, jwtToken.Claims.First(claim => claim.Type == "username").Value);
        }
    }
}
