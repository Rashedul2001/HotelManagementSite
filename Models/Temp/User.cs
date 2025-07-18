using System.ComponentModel.DataAnnotations;

namespace HotelManagementSite.Models.Temp
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;
        
        public DateTime JoinDate { get; set; } = DateTime.Now;
        
        public string Avatar { get; set; } = "/images/default-avatar.png";
    }
    
    public class UserViewModel
    {
        public List<User> Users { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = "name";
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
        public int ItemsPerPage { get; set; } = 4;
    }
}
