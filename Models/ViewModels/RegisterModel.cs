using System.ComponentModel.DataAnnotations;
namespace HotelManagementSite.Models.ViewModels{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "You must agree to the terms.")]
        public bool AcceptTerms { get; set; } = false;
    }
}