using System.ComponentModel.DataAnnotations;
namespace HotelManagementSite.Models.ViewModels
{
    public class LogInModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [Display(Name = "User Name")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}