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
        Task<IdentityUser?> FindUserByEmailAsync(string email);
        Task<IdentityUser?> GetIdentityUser(ClaimsPrincipal user);
        Task<IdentityUser?> GetIdentityUser(string email);
        Task<IEnumerable<IdentityUser>> GetAllIdentityUser();
        Task<string> GetUserRole(string identityId);
        
        // Add new method for admin user creation
        Task<IdentityResult> CreateUserAsync(string email, string password, string name, string? role = "User");

        // Add methods for user management
        Task<IdentityUser?> FindUserByIdAsync(string identityId);
        Task<IdentityResult> UpdateUserEmailAsync(string identityId, string newEmail);
        Task<IdentityResult> UpdateUserRoleAsync(string identityId, string currentRole, string newRole);
        Task<IdentityResult> DeleteUserAsync(string identityId);
    }
}