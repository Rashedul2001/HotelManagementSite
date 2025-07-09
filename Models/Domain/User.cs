// Most of the properties for declared null as I have integrated external login providers like Google, Facebook, LinkedIn, and GitHub.
// These properties are not always available while login through external providers. After login Users need to fill these details in the profile section.

using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public required string IdentityId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Email { get; set; }
		public string? NID { get; set; }
		public DateOnly? DateOfBirth { get; set; } 
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public byte[]? ProfileImage { get; set; }
		public string? ProfileImageType { get; set; }
		public string? About { get; set; }
		public ICollection<Account>? Accounts { get; set; }
		public ICollection<Booking>? Bookings { get; set; }
		public ICollection<Review>? Reviews { get; set; }
		public int VerificationTokenId { get; set; } = 0;
		public VerificationToken? VerificationToken { get; set; } = null;
	}
}
