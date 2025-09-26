using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;
using HotelManagementSite.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HotelManagementSite.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSite.Repositories
{
    public class AuthAccountRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager ): IAuthAccountRepository
    {
        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            model.UserName = await GetUniqueUserNameAsync(model.UserName);
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
            var user = await userManager.FindByNameAsync(model.EmailOrUserName) ??
                       await userManager.FindByEmailAsync(model.EmailOrUserName);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid login attempt." });
            }
            var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
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
        public AuthenticationProperties GetConfigExtAuthProp(string provider, string? returnUrl = null)
        {
            return signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        }
        public async Task<ExternalLoginInfo> GetExtLogInfoAsync()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            return info;
        }
        public async Task<SignInResult> ExternalLogInSignInAsync(ExternalLoginInfo info)
        {
            return await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        }

        public async Task<(IdentityUser? user, bool isNewUser)> FindOrCreateUserExternalAsync(ExternalLoginInfo info)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return (null, false);
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                var userName = await GetUniqueUserNameAsync(name);

                user = new IdentityUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
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

        public async Task<string> GetUserIdentityId(ClaimsPrincipal user)
        {
            var identityUser = await userManager.GetUserAsync(user);
            return identityUser?.Id ?? string.Empty;

        }
        public async Task<string> GetUniqueUserNameAsync(string? name)
        {
            var userName = HelperClass.CreateSafeUserName(name);
            while (await userManager.FindByNameAsync(userName) != null)
            {
                userName = HelperClass.GenerateUniqueUserName(name);
            }
            return userName;
        }
        public async Task<IdentityUser?> FindUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        // Add new method for admin user creation
        public async Task<IdentityResult> CreateUserAsync(string email, string password, string name, string? role = "User")
        {
            var userName = await GetUniqueUserNameAsync(name);

            var identityUser = new IdentityUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(identityUser, password);

            if (result.Succeeded && !string.IsNullOrEmpty(role))
            {
                await userManager.AddToRoleAsync(identityUser, role);
            }

            return result;
        }

        public async Task<IdentityUser?> GetIdentityUser(ClaimsPrincipal user)
        {
            return await userManager.GetUserAsync(user);
        }
        public async Task<IdentityUser?> GetIdentityUser(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<IdentityUser>> GetAllIdentityUser()
        {
            return await userManager.Users.ToListAsync();
        }

        public async Task<string> GetUserRole(string identityId)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == identityId);
            if (user == null)
            {
                return "Unknown";
            }
            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "User";

        }
    }
}