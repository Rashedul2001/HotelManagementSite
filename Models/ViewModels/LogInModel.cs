using System.ComponentModel.DataAnnotations;
namespace HotelManagementSite.Models.ViewModels
{
    public class LogInModel
    {
        [Required(ErrorMessage = "This Field is required")]
        [Display(Name = "User Name or Email")]
        public required string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; } = false;
    }
}