using System.ComponentModel.DataAnnotations;

namespace BE_Mobile.DTO
{
	public class TokenRequest
	{
		public string? SignedToken { get; set; }
	}
}
