using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
namespace HotelManagementSite.Interfaces{
    public interface IAuthAccountRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task<IdentityResult> LoginAsync(LogInModel model);
        Task LoginAsync(IdentityUser user, bool isPersistent);
		Task LogoutAsync();
        AuthenticationProperties GetConfigExtAuthProp(string provider, string? returnUrl);
        Task<ExternalLoginInfo> GetExtLogInfoAsync(); 
        Task<SignInResult> ExternalLogInSignInAsync(ExternalLoginInfo info);
        Task<(IdentityUser? user, bool isNewUser)> FindOrCreateUserExternalAsync(ExternalLoginInfo info);
        Task<string> GetUserIdentityId(ClaimsPrincipal user);
        Task<string> GetUniqueUserNameAsync(string name);
        





    }

}