using System.ComponentModel.DataAnnotations;
namespace HotelManagementSite.Models.ViewModels{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public  string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public  string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public  string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "You must agree to the terms.")]
        public bool AcceptTerms { get; set; } = false;
    }
}