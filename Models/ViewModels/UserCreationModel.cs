using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.ViewModels
{
    public class UserCreationModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = "User";

        [StringLength(20, ErrorMessage = "NID cannot exceed 20 characters")]
        public string? NID { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "About cannot exceed 500 characters")]
        public string? About { get; set; }

        public IFormFile? ProfileImage { get; set; }

        // Internal properties for processing
        public byte[]? ProfileImageBytes { get; set; }
        public string? ProfileImageType { get; set; }
    }
}
