using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Domain{
    public class Review{
        [Key]
        public int Id { get; set; }
        [Range(1, 5)]
        [Display(Name = "Rating (1-5)")]
        [Required(ErrorMessage = "Please provide a rating between 1 and 5.")]
        public required int Rating { get; set; } = 0;
        [Required(ErrorMessage = "The Content field is required.")]
        [MaxLength(500, ErrorMessage = "The Content field cannot exceed 500 characters.")]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int HotelRoomId { get; set; }
        public HotelRoom HotelRoom { get; set; } 
        public int UserId { get; set; }
        public User User { get; set; } 
    }
}