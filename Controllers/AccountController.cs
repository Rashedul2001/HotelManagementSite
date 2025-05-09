using HotelManagementSite.interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.Controllers
{
    public class AccountController(IAuthRepository auth) : Controller
    {
        private readonly IAuthRepository auth = auth;
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await auth.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }
        
    }
}
