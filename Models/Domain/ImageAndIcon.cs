using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain{

    public class ImageAndIcon{
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Category { get; set; } // e.g., "HotelRoom", "Profile", "Icon"
        [Required]
        [MaxLength(100)]
        public required string FileName { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Image data cannot be empty.")]
        public required byte[] ImageData { get; set; }
        public int? HotelRoomId { get; set; }
        public HotelRoom? HotelRoom { get; set; } 
        [Required]
        public string ContentType {get;set;} // e.g., "image/png", "image/jpeg"
        [MaxLength(255)]
        public string? Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}