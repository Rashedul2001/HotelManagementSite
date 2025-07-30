using System.Security.Claims;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.ViewComponents
{
    public class UserPictureViewComponent(IUserRepository userRepo, IAuthAccountRepository authRepo) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return View(null);
            }
            var identityId = await authRepo.GetUserIdentityId((ClaimsPrincipal)User);
            var user = await userRepo.GetUserByIdentityIdAsync(identityId);
            if (user == null)
            {
                return View(null);
            }
            var userIcon = new UserPictureViewModel
            {
                Name = user.Name,
                ProfileImage = user.ProfileImage,
                ProfileImageType = user.ProfileImageType
            };
            return View(userIcon);
            

        }

    }

}