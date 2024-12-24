using BE_Mobile.Context;
using BE_Mobile.DTO;
using BE_Mobile.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE_Mobile.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticateController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		public AuthenticateController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> Authenticate([FromBody] TokenRequest tokenRequest)
		{
			var idToken = tokenRequest.SignedToken;

			try
			{
				// Xác minh ID Token và lấy thông tin từ payload
				var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
				string email = payload.Email;
				string googleId = payload.Subject; // ID duy nhất của tài khoản Google
				string name = payload.Name;

				// Kiểm tra xem người dùng đã tồn tại trong hệ thống chưa
				var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == email);

				if (existingUser != null)
				{
					// Người dùng đã tồn tại, trả về thông tin
					return Ok(new
					{
						message = "Authentication successful",
						user = new
						{
							id = existingUser.Id,
							email = existingUser.Email,
							nickname = existingUser.Nickname,
						}
					});
				}

				// Nếu không tồn tại, tạo tài khoản mới
				var newUser = new User
				{
					Email = email,
					Nickname = name,
					GoogleId = googleId,
					CreatedAt = DateTime.UtcNow
				};

				_context.Users.Add(newUser);
				await _context.SaveChangesAsync();

				return Ok(new
				{
					message = "Account created and authentication successful",
					user = new
					{
						id = newUser.Id,
						email = newUser.Email,
						nickname = newUser.Nickname,
					}
				});
			}
			catch (InvalidJwtException ex)
			{
				// ID Token không hợp lệ
				return Unauthorized(new { message = "Invalid Google ID Token", error = ex.Message });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi chung
				return StatusCode(500, new { message = "An error occurred during authentication", error = ex.Message });
			}
		}

	}
}
