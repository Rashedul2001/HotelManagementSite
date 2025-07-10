
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagementSite.Helpers;
namespace HotelManagementSite.Controllers
{

	public class AccountController(IAuthAccountRepository authAcRepo) : Controller
	{
		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				model.UserName = HelperClass.CreateSafeUserName(model.UserName);
				var result = await authAcRepo.RegisterAsync(model);
				if (result.Succeeded)
				{
					TempData["SuccessMessage"] = "Registration Successful. You can now log in.";
					return RedirectToAction("Index", "Home");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			TempData["Info"] = "ShowRegisterModal";
			TempData["ErrorMessage"] = "Registration Failed. Please Try Again";
			return RedirectToAction("Index", "Home");
		}
		[HttpGet]
		public IActionResult LogIn()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				return RedirectToAction("Profile");
			}
			TempData["ErrorMessage"] = "LogIn Required";
			TempData["info"] = "ShowLogInModal";
			return RedirectToAction("Index", "Home");

		}
		[HttpPost]
		public async Task<IActionResult> LogIn(LogInModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await authAcRepo.LoginAsync(model);
				if (result.Succeeded)
				{
					TempData["SuccessMessage"] = "LogIn Successful";
					return RedirectToAction("Profile");
				}
				TempData["ErrorMessage"] = "Invalid Username or Password. Please Try Again.";
			}
			TempData["Info"] = "ShowLogInModal";
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		public IActionResult Profile()
		{
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await authAcRepo.LogoutAsync();
			TempData["SuccessMessage"] = "You have been logged out successfully.";
			return RedirectToAction("Index", "Home");
		}
		public IActionResult ExternalLogIn(string provider, string? returnUrl = null)
		{
			var redirectUrl = Url.Action("ExternalLogInCallback", "Account", new { returnUrl }) ?? Url.Action("Index", "Home");
			var properties = authAcRepo.GetConfigExtAuthProp(provider, redirectUrl);
			properties.Items["prompt"] = "select_account";
			return Challenge(properties, provider);
		}
		public async Task<IActionResult> ExternalLogInCallback(string? returnUrl = null, string? remoteError = null)
		{
			if (remoteError != null)
			{
				TempData["ErrorMessage"] = "External login error: " + remoteError;
				return RedirectToAction("LogIn");
			}
			var info = await authAcRepo.GetExtLogInfoAsync();
			if (info == null)
			{
				TempData["ErrorMessage"] = "External login information not found.";
				return RedirectToAction("LogIn");
			}
			var result = await authAcRepo.ExternalLogInSignInAsync(info);
			if (result.Succeeded)
			{
				TempData["SuccessMessage"] = "External login successful.";

				return RedirectToLocal(returnUrl);
			}
			var (user, isNewUser) = await authAcRepo.FindOrCreateUserExternalAsync(info);
			if (user != null)
			{
				await authAcRepo.LoginAsync(user, isPersistent: false);
				if (isNewUser)
				{
					TempData["SuccessMessage"] = "Welcome! Your account has been created successfully.";
				}
				else
				{
					TempData["SuccessMessage"] = "Welcome back! You have logged in successfully.";
				}
				return RedirectToAction("Profile");
			}
			TempData["ErrorMessage"] = "External login failed. Please try again.";
			TempData["Info"] = "ShowLogInModal";
			return RedirectToAction("Index", "Home");
		}

		private IActionResult RedirectToLocal(string? returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			TempData["ErrorMessage"] = "Invalid return URL.";
			return RedirectToAction("Index", "Home");
		}
		public IActionResult AccessDenied()
		{
			TempData["ErrorMessage"] = "You do not have permission to access this page.";
			return View();
		}


	}
}
