using BE_Mobile.Context;
using BE_Mobile.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BE_Mobile.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] User user)
		{
			var existingUser = await _context.Users
				.FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
			if (existingUser != null)
			{
				return BadRequest("Username or email already exists.");
			}

			user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);  // Mã hóa mật khẩu
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Registration successful" });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] User user)
		{
			var existingUser = await _context.Users
				.FirstOrDefaultAsync(u => u.Username == user.Username);
			if (existingUser == null || !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
			{
				return Unauthorized("Invalid username or password");
			}

			// Tạo token hoặc session nếu cần
			return Ok(new { message = "Login successful" });
		}

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var username = User.Identity.Name; // Lấy username từ token
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new { user.Username, user.Email });
        }

        // Tạo JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Lấy Key từ cấu hình
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
