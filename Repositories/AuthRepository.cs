using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;
using HotelManagementSite.interfaces;
namespace HotelManagementSite.Repositories
{
    public class AuthRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAuthRepository
    {
        readonly UserManager<IdentityUser> userManager = userManager;
        readonly SignInManager<IdentityUser> signInManager = signInManager;
        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
            return result;
        }
        public async Task<IdentityResult> LoginAsync(LogInModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." });
            }
        }
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }


    }

}