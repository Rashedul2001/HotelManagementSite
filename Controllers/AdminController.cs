using HotelManagementSite.Helpers;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSite.Controllers
{
    public class AdminController(IAuthAccountRepository authAccountRepository) : Controller
    {
        private readonly IAuthAccountRepository authRepo = authAccountRepository;

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult DashBoard()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> CreateUser(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserName = HelperClass.CreateSafeUserName(model.UserName);
                var result = await authRepo.RegisterAsync(model);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User created successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            TempData["ErrorMessage"] = "User creation failed. Please try again.";
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult SuperAdminPage()
        {
            return View();

        }
    }
}