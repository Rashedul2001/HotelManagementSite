using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;
using HotelManagementSite.interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HotelManagementSite.Helpers;
namespace HotelManagementSite.Repositories
{
    public class AuthRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : IAuthRepository
    {
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
        public async Task LoginAsync(IdentityUser user, bool isPersistent)
        {
            await signInManager.SignInAsync(user, isPersistent);
		}

		public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }
        public  AuthenticationProperties GetConfigExtAuthProp(string provider, string? returnUrl = null)
        {
            return signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        }
        public async Task<ExternalLoginInfo> GetExtLogInfoAsync()
        {
            var info = await signInManager.GetExternalLoginInfoAsync() ?? throw new InvalidOperationException("External login info could not be retrieved.");
            return info;
        }
        public async Task<SignInResult> ExternalLogInSignInAsync(ExternalLoginInfo info)
        {
            return await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        }

        public async Task<(IdentityUser? user, bool isNewUser)> FindOrCreateExternalUserAsync(ExternalLoginInfo info)
        {
            var emailClaim = info.Principal.FindFirst(ClaimTypes.Email);
            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
            {
                return (null, false);
            }
            var email = emailClaim.Value;
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                name = HelperClass.CreateSafeUserName(name);
				user = new IdentityUser
                {
                    UserName = name,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return (null, false);
                await userManager.AddToRoleAsync(user, "User");
                await userManager.AddLoginAsync(user, info);
                return (user, true);
            }
            else
            {
                var logins = await userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                {
                    await userManager.AddLoginAsync(user, info);
                }
                return (user, false);
            }
        }






   

    }

}