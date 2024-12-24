using BE_Mobile.Context;
using BE_Mobile.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_Mobile.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public AuthController(ApplicationDbContext context)
		{
			_context = context;
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
	}

}
