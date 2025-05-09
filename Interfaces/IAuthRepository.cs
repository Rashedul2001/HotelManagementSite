using Microsoft.AspNetCore.Identity;
using HotelManagementSite.Models.ViewModels;

namespace HotelManagementSite.interfaces{
    public interface IAuthRepository
    {
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task<IdentityResult> LoginAsync(LogInModel model);
        Task LogoutAsync();


    }

}