using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Temp
{
    public class Room
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Room number is required")]
        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
        public string Number { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Room type is required")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }
        
        [Required(ErrorMessage = "Floor is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Floor must be at least 1")]
        public int Floor { get; set; }
        
        public List<string> Amenities { get; set; } = new();
        
        [Display(Name = "Amenities")]
        public string AmenitiesString { get; set; } = string.Empty;
    }
    
    public class RoomViewModel
    {
        public List<Room> Rooms { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "number";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalRooms { get; set; }
        public int ItemsPerPage { get; set; } = 4;
    }
}
