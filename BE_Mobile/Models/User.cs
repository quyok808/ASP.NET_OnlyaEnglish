namespace BE_Mobile.Models
{
	public class User
	{
		public int Id { get; set; } // ID nội bộ
		public string? Username { get; set; } // Tùy chọn, nếu muốn hỗ trợ đăng nhập thông thường
		public string? Password { get; set; } // Chỉ sử dụng nếu hỗ trợ đăng nhập bằng username/password
		public string Email { get; set; } // Email là bắt buộc (Google trả về email)
		public string? Nickname { get; set; } // Biệt danh, nếu cần hiển thị
		public string? GoogleId { get; set; } // Lưu Google "sub" từ ID Token
		public string? Provider { get; set; } // Tên provider (vd: "Google", để hỗ trợ đa provider)
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo
	}
}
