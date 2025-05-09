using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain
{
	public class Booking
	{
		[Key]
		public int Id { get; set; }

		public DateOnly CheckInDate { get; set; }
		public DateOnly CheckOutDate { get; set; }
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Number of Days is required")]
		public int NumberOfDays { get; set; } = 1;
		[Required]
		[Range(0, int.MaxValue)]
		public int Discount { get; set; } = 0;
		[Required]
		[Range(1, int.MaxValue)]
		public int Adults { get; set; } = 1;
		[Required]
		[Range(0, int.MaxValue)]
		public int Children { get; set; } = 0;

		[Required]
		[Range(0, int.MaxValue)]
		public int TotalPrice { get; set; } = 0;

		public int UserId { get; set; }
		public required User User { get; set; }
		public int HotelRoomId { get; set; }
		[Required]
		public required HotelRoom HotelRoom { get; set; }







	}
}
