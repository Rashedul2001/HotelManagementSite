using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
namespace HotelManagementSite.interfaces{
    public interface IAuthRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task<IdentityResult> LoginAsync(LogInModel model);
        Task LoginAsync(IdentityUser user, bool isPersistent);
		Task LogoutAsync();
        AuthenticationProperties GetConfigExtAuthProp(string provider, string? returnUrl);
        Task<ExternalLoginInfo> GetExtLogInfoAsync(); 
        Task<SignInResult> ExternalLogInSignInAsync(ExternalLoginInfo info);
        Task<(IdentityUser? user, bool isNewUser)> FindOrCreateExternalUserAsync(ExternalLoginInfo info);




    }

}