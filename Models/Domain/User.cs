using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.Models.Domain
{
	public class User
	{
		[Key]
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? NID { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public byte[]? ProfileImage { get; set; }
		public string? About { get; set; }
		public ICollection<Account> Accounts { get; set; }
		public ICollection<Booking> Bookings { get; set; }
		public ICollection<Review> Reviews { get; set; }
		public int VerificationTokenId { get; set; }
		public VerificationToken VerificationToken { get; set; } = null!;
	}
}
