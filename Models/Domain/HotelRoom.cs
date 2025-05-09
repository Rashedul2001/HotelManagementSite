using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HotelManagementSite.Models.Domain
{
	public class HotelRoom
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(50, ErrorMessage = "The Name field cannot exceed 50 characters.")]
		public required string Name { get; set; }
		[Required(ErrorMessage = "The slug Field is Required")]
		[MaxLength(50, ErrorMessage = "The Slug field cannot exceed 50 characters.")]
		public string? Slug { get; set; }
		public void GenerateSlug()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				Slug = Regex.Replace(Name.ToLower(), @"\s+", "-");
				Slug = Regex.Replace(Slug, @"[^a-z0-9\-]", "");
			}
		}
		[Required]
		[Range(100, int.MaxValue, ErrorMessage = "Length must be greater than 100")]
		public string? Description { get; set; }
		[Required]
		[Range(200, int.MaxValue, ErrorMessage = "The Price field must be a positive number.")]
		public required int Price { get; set; } = 1;
		[Range(0, int.MaxValue, ErrorMessage = "The Discount field must be a non negative number.")]
		public int Discount { get; set; } = 0;
		public ICollection<ImageAndIcon> HotelRoomImages { get; set; }
		public byte[]? CoverImage { get; set; }
		public string RoomType { get; set; } = "Basic";
		public string SpecialNote { get; set; } = "Check In Time: 12:00 PM, Check Out Time: 11:00 AM. If you leave behind any items, please contact the receptionist.";
		public string Dimensions { get; set; } = "Length: 10ft, Width: 10ft, Height: 10ft";
		public int NumberOfBeds { get; set; } = 1;
		public ICollection<Amenity> Amenities { get; set;}
		public Boolean IsBooked { get; set; } = false;
		public Boolean IsFeatured { get; set; } = true;
		public ICollection<Review> Reviews { get; set; } 
		public ICollection<Booking> Bookings { get; set; }


	}
}
