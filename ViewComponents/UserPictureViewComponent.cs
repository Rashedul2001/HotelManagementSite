using System.Security.Claims;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.ViewComponents
{
    public class UserPictureViewComponent(IUserHotelRepository userRepo, IAuthAccountRepository authRepo) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var identityId = await authRepo.GetUserIdentityId((ClaimsPrincipal)User);
            
            if (string.IsNullOrEmpty(identityId))
            {
                // User is not authenticated, return default
                return View(new UserPictureViewModel { Name = "Guest" });
            }

            // Try to get hotel user data
            var user = await userRepo.GetUserByIdentityIdAsync(identityId);
            
            if (user != null)
            {
                // User exists in hotel database
                var userIcon = new UserPictureViewModel
                {
                    Name = user.Name,
                    ProfileImage = user.ProfileImage,
                    ProfileImageType = user.ProfileImageType
                };
                return View(userIcon);
            }
            else
            {
                // User doesn't exist in hotel database, get identity user info as fallback
                var identityUser = await authRepo.GetIdentityUser((ClaimsPrincipal)User);
                var fallbackName = identityUser?.UserName ?? identityUser?.Email ?? "User";
                
                return View(new UserPictureViewModel { Name = fallbackName });
            }
        }
    }
}