using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain
{
	public class Account
	{
		[Key]
		public int Id { get; set; }
		public string? ProviderType { get; set; }
		public string? ProviderId { get; set; }
		public string? ProviderAccountId { get; set; }
		public string? RefreshToken { get; set; }
		public string? AccessToken { get; set; }
		public int AccessTokenExpires { get; set; }
		public int UserId { get; set; }
		public User? User { get; set; }
	}
}
