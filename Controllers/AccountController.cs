using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelManagementSite.Helpers;
using Microsoft.Extensions.Logging;
using HotelManagementSite.Models.Domain;
using System.Security.Claims;

namespace HotelManagementSite.Controllers
{

	public class AccountController(IAuthAccountRepository authAcRepo, IUserHotelRepository userHtlRepo, ILogger<AccountController> logger) : Controller
	{
		[HttpPost]
		public async Task<IActionResult> Register(RegisterModel model)
		{
			// not registering the user into HotelDb 
			if (ModelState.IsValid)
			{
				if (await authAcRepo.FindUserByEmailAsync(model.Email) != null)
				{
					ModelState.AddModelError(string.Empty, "Email is already in use.");
					TempData["Info"] = "ShowRegisterModal";
					TempData["ErrorMessage"] = "User with this email already exists.";
					return View(model);
				}
				var result = await authAcRepo.RegisterAsync(model);
				if (result.Succeeded)
				{
					var user = await authAcRepo.GetIdentityUser(model.Email);
					await userHtlRepo.CreateUserAsync(user);
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
		public async Task<IActionResult> Profile(string? adminPro)
		{
			// later fix the issue with multiple accounts
			// make it so that it goes to only one profile with multiple accounts
			try
			{
				var identityUser = await authAcRepo.GetIdentityUser(User);
				if (identityUser == null)
				{
					await authAcRepo.LogoutAsync();
					TempData["ErrorMessage"] = "User not found. Please log in again.";
					logger.LogWarning("User not found in the Identity system during profile access. Logged out for security.");
					return RedirectToAction("Index", "Home");
				}
				var user = await userHtlRepo.GetUserByIdentityIdAsync(identityUser.Id);
				if (user == null)
				{
					user = new User
					{
						IdentityId = identityUser.Id,
						Name = identityUser.UserName ?? "User",
						Email = identityUser.Email ?? "",
					};
				}
				var profileInfo = new ProfileModel
				{
					Id = user.Id,
					Name = user.Name,
					UserName = identityUser.UserName,
					Email = identityUser.Email ?? "",
					NID = user.NID,
					DateOfBirth = user.DateOfBirth,
					PhoneNumber = identityUser.PhoneNumber,
					Address = user.Address,
					About = user.About,
					ProfileImage = user.ProfileImage,
					ProfileImageType = user.ProfileImageType,
					Role = await authAcRepo.GetUserRole(identityUser.Id),
					Accounts = user.Accounts ?? new List<Account>(),
					Bookings = user.Bookings ?? new List<Booking>(),
					Reviews = user.Reviews ?? new List<Review>(),
					TwoFactorEnabled = identityUser.TwoFactorEnabled,
					MobileNumberVerified = identityUser.PhoneNumberConfirmed,
					EmailVerified = identityUser.EmailConfirmed
				};

				if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
				{
					// if admin or superAdmin click to the homepage user icon redirect to dashboard
					// if profile is clicked from dashboard redirect to profile view
					if (adminPro == "DashBoard")
					{
						ViewBag.CurrentAction = "Profile";
						return View(profileInfo);
					}
					return RedirectToAction("Index", "Admin");
				}

				// if a user clicked user icon on the homepage redirect to profile view
				return View(profileInfo);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error occurred while fetching user profile.");
				TempData["ErrorMessage"] = "An error occurred while fetching your profile. Please try again later.";
				return RedirectToAction("Index", "Home");
			}

		}
		[Authorize]
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

				await userHtlRepo.AddExternalUserAsync(info, user.Id);
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
			return View();
		}


	}
}
