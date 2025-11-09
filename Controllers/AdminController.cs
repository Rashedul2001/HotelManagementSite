using System.Globalization;
using System.Transactions;
using HotelManagementSite.Helpers;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Models.Domain;
using HotelManagementSite.Models.Temp;
using HotelManagementSite.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HotelManagementSite.Controllers
{
    public class AdminController(IAuthAccountRepository authAcRepo, IUserHotelRepository userHtlRepo) : Controller
    {

		[Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Index()
        {
            ViewBag.CurrentAction = "Index";
            return View();
        }

		[HttpGet]
		public async Task<IActionResult> GetUser(int id)
		{
			try
			{
				var user = await userHtlRepo.GetUserByIdAsync(id);
				if (user == null)
				{
					return Json(new
					{
						success = false,
						message = "User not found"
					});
				}

				var role = await authAcRepo.GetUserRole(user.IdentityId);

				var userData = new
				{
					id = user.Id,
					name = user.Name,
					email = user.Email,
					role = role,
					nid = user.NID,
					phoneNumber = user.PhoneNumber,
					dateOfBirth = user.DateOfBirth?.ToString("yyyy-MM-dd"),
					address = user.Address,
					about = user.About,
					profileImage = user.ProfileImage != null ? Convert.ToBase64String(user.ProfileImage) : null,
					profileImageType = user.ProfileImageType
				};

				return Json(new
				{
					success = true,
					user = userData
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = "An error occurred while retrieving user data"
				});
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser([FromForm] int id, [FromForm] EditUserModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList();

					return Json(new
					{
						success = false,
						message = "Validation failed",
						errors
					});
				}

				// Get existing user
				var existingUser = await userHtlRepo.GetUserByIdAsync(id);
				if (existingUser == null)
				{
					return Json(new
					{
						success = false,
						message = "User not found"
					});
				}

				// Check if email is being changed and if new email already exists
				if (!existingUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
				{
					var emailExists = await authAcRepo.FindUserByEmailAsync(model.Email);
					if (emailExists != null)
					{
						return Json(new
						{
							success = false,
							message = "Email address is already in use by another user"
						});
					}
				}

				// Handle profile image
				byte[]? profileImage = existingUser.ProfileImage;
				string? profileImageType = existingUser.ProfileImageType;

				if (model.ProfileImage != null && model.ProfileImage.Length > 0)
				{
					// Validate image file
					var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
					if (!allowedTypes.Contains(model.ProfileImage.ContentType.ToLower()))
					{
						return Json(new
						{
							success = false,
							message = "Invalid image format. Only JPEG, PNG, and GIF are allowed."
						});
					}

					// Check file size (limit to 5MB)
					if (model.ProfileImage.Length > 5 * 1024 * 1024)
					{
						return Json(new
						{
							success = false,
							message = "Image file size cannot exceed 5MB."
						});
					}

					using var memoryStream = new MemoryStream();
					await model.ProfileImage.CopyToAsync(memoryStream);
					profileImage = memoryStream.ToArray();
					profileImageType = model.ProfileImage.ContentType;
				}

				// Update Identity user info
				var identityUser = await authAcRepo.FindUserByIdAsync(existingUser.IdentityId);
				if (identityUser != null)
				{
					// Update email if changed
					if (!identityUser.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
					{
						var updateEmailResult = await authAcRepo.UpdateUserEmailAsync(identityUser.Id, model.Email);
						if (!updateEmailResult.Succeeded)
						{
							return Json(new
							{
								success = false,
								message = "Failed to update user email",
								errors = updateEmailResult.Errors.Select(e => e.Description).ToList()
							});
						}
					}

					// Update role if changed
					var currentRole = await authAcRepo.GetUserRole(identityUser.Id);
					if (!currentRole.Equals(model.Role, StringComparison.OrdinalIgnoreCase))
					{
						var updateRoleResult = await authAcRepo.UpdateUserRoleAsync(identityUser.Id, currentRole, model.Role);
						if (!updateRoleResult.Succeeded)
						{
							return Json(new
							{
								success = false,
								message = "Failed to update user role",
								errors = updateRoleResult.Errors.Select(e => e.Description).ToList()
							});
						}
					}
				}

				// Update hotel user
				var updatedUser = await userHtlRepo.UpdateUserAsync(
					id,
					model.Name,
					model.Email,
					model.NID,
					model.DateOfBirth,
					model.PhoneNumber,
					model.Address,
					model.About,
					profileImage,
					profileImageType
				);

				return Json(new
				{
					success = true,
					message = $"User '{model.Name}' updated successfully!",
					user = new
					{
						id = updatedUser.Id,
						name = updatedUser.Name,
						email = updatedUser.Email,
						role = model.Role,
						phoneNumber = updatedUser.PhoneNumber ?? "Not Provided",
						address = updatedUser.Address ?? "No Address Provided"
					}
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = "An error occurred while updating the user. Please try again."
				});
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteUser(int id)
		{
			try
			{
				var user = await userHtlRepo.GetUserByIdAsync(id);
				if (user == null)
				{
					return Json(new
					{
						success = false,
						message = "User not found"
					});
				}

				// Delete from hotel database first
				var deleteResult = await userHtlRepo.DeleteUserAsync(id);
				if (!deleteResult)
				{
					return Json(new
					{
						success = false,
						message = "Failed to delete user from hotel database"
					});
				}

				// Delete from Identity database
				var identityDeleteResult = await authAcRepo.DeleteUserAsync(user.IdentityId);
				if (!identityDeleteResult.Succeeded)
				{
					return Json(new
					{
						success = false,
						message = "Failed to delete user from authentication system",
						errors = identityDeleteResult.Errors.Select(e => e.Description).ToList()
					});
				}

				return Json(new
				{
					success = true,
					message = $"User '{user.Name}' deleted successfully!"
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = "An error occurred while deleting the user. Please try again."
				});
			}
		}

		[HttpGet]
		[Authorize(Roles = "Admin,SuperAdmin")]
		public async Task<IActionResult> ViewUserDetails(int id)
		{
			try
			{
				var user = await userHtlRepo.GetUserByIdAsync(id);
				if (user == null)
				{
					TempData["Error"] = "User not found";
					return RedirectToAction("Users");
				}

				var identityUser = await authAcRepo.FindUserByIdAsync(user.IdentityId);
				if (identityUser == null)
				{
					TempData["Error"] = "User identity not found";
					return RedirectToAction("Users");
				}

				// Build the profile model for this user
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

				ViewBag.IsViewingOtherUser = true;
				ViewBag.CurrentAction = "Users";
				
				return View("~/Views/Account/Profile.cshtml", profileInfo);
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while retrieving user details";
				return RedirectToAction("Users");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser([FromForm] UserCreationModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values
						.SelectMany(v => v.Errors)
						.Select(e => e.ErrorMessage)
						.ToList();

					return Json(new
					{
						success = false,
						message = "Validation failed",
						errors
					});
				}

				// Check if user already exists
				var existingUser = await  authAcRepo.FindUserByEmailAsync(model.Email);
				if (existingUser != null)
				{
					return Json(new
					{
						success = false,
						message = "User with this email already exists"
					});
				}

				// Handle profile image
				byte[]? profileImage = null;
				string? profileImageType = null;

				if (model.ProfileImage != null && model.ProfileImage.Length > 0)
				{
					// Validate image file
					var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
					if (!allowedTypes.Contains(model.ProfileImage.ContentType.ToLower()))
					{
						return Json(new
						{
							success = false,
							message = "Invalid image format. Only JPEG, PNG, and GIF are allowed."
						});
					}

					// Check file size (limit to 5MB)
					if (model.ProfileImage.Length > 5 * 1024 * 1024)
					{
						return Json(new
						{
							success = false,
							message = "Image file size cannot exceed 5MB."
						});
					}

					using var memoryStream = new MemoryStream();
					await model.ProfileImage.CopyToAsync(memoryStream);
					profileImage = memoryStream.ToArray();
					profileImageType = model.ProfileImage.ContentType;
				}

				// Create Identity user
				var identityResult = await authAcRepo.CreateUserAsync(model.Email, model.Password, model.Name, model.Role);

				if (!identityResult.Succeeded)
				{
					var errors = identityResult.Errors.Select(e => e.Description).ToList();
					return Json(new
					{
						success = false,
						message = "Failed to create user account",
						errors
					});
				}

				// Get the created identity user
				var identityUser = await authAcRepo.FindUserByEmailAsync(model.Email);
				if (identityUser == null)
				{
					return Json(new
					{
						success = false,
						message = "User creation failed"
					});
				}

				// Create hotel user
				var hotelUser = await userHtlRepo.AddUserAsync(
					identityUser.Id,
					model.Name,
					model.Email,
					model.NID,
					model.DateOfBirth,
					model.PhoneNumber,
					model.Address,
					model.About,
					profileImage,
					profileImageType
				);

				return Json(new
				{
					success = true,
					message = $"User '{model.Name}' created successfully!",
					user = new
					{
						id = hotelUser.Id,
						name = hotelUser.Name,
						email = hotelUser.Email,
						role = model.Role
					}
				});
			}
			catch (Exception ex)
			{
				// Log the exception here
				return Json(new
				{
					success = false,
					message = "An error occurred while creating the user. Please try again."
				});
			}
		}
        // this is called initially to load the view with default model 
		// Used in the Hotel Users page
        public IActionResult Users()
		{
			ViewBag.CurrentAction = "Users";
			
			// Create a default model with initial values for the form
			var defaultModel = new PagedResult<UserModel>
			{
				Items = [],
				CurrentPage = 1,
				PageSize = 5,
				TotalPages = 0,
				TotalItems = 0,
				StartItemIndex = 0,
				EndItemIndex = 0,
				SearchTerm = "",
				SortBy = "name",
				SortOrder = "asc"
			};
			
			return View(defaultModel);
		}
		[HttpGet]
		public async Task<IActionResult> GetUsers(
			int currentPage = 1,
			int pageSize = 5,
			string searchTerm = "",
			string sortBy = "name",
			string sortOrder = "asc")
		{
			try
			{
				System.Diagnostics.Debug.WriteLine($"GetUsers called with: Page={currentPage}, PageSize={pageSize}, SearchTerm='{searchTerm}', SortBy={sortBy}, SortOrder={sortOrder}");

				var query = userHtlRepo.GetAllUsersAsQueryable();
				bool isRoleBasedSearch = false;

				// Check if search term might be role-related
				if (!string.IsNullOrWhiteSpace(searchTerm))
				{
					var term = searchTerm.Trim().ToLower();
					var commonRoles = new[] { "admin", "superadmin", "user", "manager", "staff", "guest" };
					isRoleBasedSearch = commonRoles.Any(role => role.Contains(term) || term.Contains(role));
				}

				List<User> userList;
				int totalUsers;
				int totalPages;

                // TODO: Optimize role fetching by caching roles if necessary
                // TODO: Can be optimized by joining with role with user from both databse to a single table and here should be used a third repository for accessing both databases


                // If it's potentially a role-based search, get all users first to avoid missing matches
                if (isRoleBasedSearch)
				{
					// For role-based searches, we need to get all users to check their roles
					var allUsers = await query.ToListAsync();
					var allUserModels = new List<UserModel>();

					foreach (var user in allUsers)
					{
						var role = await authAcRepo.GetUserRole(user.IdentityId);
						allUserModels.Add(new UserModel
						{
							Id = user.Id,
							Name = user.Name,
							Email = user.Email,
							Role = role,
							PhoneNumber = user.PhoneNumber ?? "Not Provided",
							Address = user.Address ?? "No Address Provided",
						});
					}

					// Apply search filter to all user models (including roles)
					if (!string.IsNullOrWhiteSpace(searchTerm))
					{
						var term = searchTerm.Trim().ToLower();
						allUserModels = allUserModels.Where(u =>
							(u.Name?.ToLower().Contains(term) ?? false) ||
							(u.Email?.ToLower().Contains(term) ?? false) ||
							(u.PhoneNumber?.ToLower().Contains(term) ?? false) ||
							(u.Address?.ToLower().Contains(term) ?? false) ||
							(u.Role?.ToLower().Contains(term) ?? false)
						).ToList();
					}

					// Handle sorting
					allUserModels = (sortBy.ToLower(), sortOrder.ToLower()) switch
					{
						("email", "asc") => [.. allUserModels.OrderBy(u => u.Email)],
						("email", "desc") => [.. allUserModels.OrderByDescending(u => u.Email)],
						("phone", "asc") => [.. allUserModels.OrderBy(u => u.PhoneNumber)],
						("phone", "desc") => [.. allUserModels.OrderByDescending(u => u.PhoneNumber)],
						("address", "asc") => [.. allUserModels.OrderBy(u => u.Address)],
						("address", "desc") => [.. allUserModels.OrderByDescending(u => u.Address)],
						("role", "asc") => [.. allUserModels.OrderBy(u => u.Role)],
						("role", "desc") => [.. allUserModels.OrderByDescending(u => u.Role)],
						("name", "desc") => [.. allUserModels.OrderByDescending(u => u.Name)],
						_ => [.. allUserModels.OrderBy(u => u.Name)]
					};

					totalUsers = allUserModels.Count;
					totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

					// Apply pagination
					var userModels = pageSize == 100 
						? allUserModels 
						: allUserModels.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

					var result = new PagedResult<UserModel>
					{
						Items = userModels,
						CurrentPage = currentPage,
						TotalPages = totalPages,
						PageSize = pageSize,
						TotalItems = totalUsers,
						StartItemIndex = totalUsers == 0 ? 0 : (currentPage - 1) * pageSize + 1,
						EndItemIndex = Math.Min(currentPage * pageSize, totalUsers),
						SortBy = sortBy,
						SortOrder = sortOrder,
						SearchTerm = searchTerm,
					};

					return PartialView("Partials/_UsersTable", result);
				}
				else
				{
					// For non-role searches, use efficient database-level filtering
					if (!string.IsNullOrWhiteSpace(searchTerm))
					{
						var term = searchTerm.Trim().ToLower();
						query = query.Where(u =>
							(u.Name != null && u.Name.ToLower().Contains(term)) ||
							(u.Email != null && u.Email.ToLower().Contains(term)) ||
							(u.PhoneNumber != null && u.PhoneNumber.ToLower().Contains(term)) ||
							(u.Address != null && u.Address.ToLower().Contains(term))
						);
					}

					// Apply database-level sorting (excluding role sorting)
					query = (sortBy.ToLower(), sortOrder.ToLower()) switch
					{
						("email", "asc") => query.OrderBy(u => u.Email),
						("email", "desc") => query.OrderByDescending(u => u.Email),
						("phone", "asc") => query.OrderBy(u => u.PhoneNumber),
						("phone", "desc") => query.OrderByDescending(u => u.PhoneNumber),
						("address", "asc") => query.OrderBy(u => u.Address),
						("address", "desc") => query.OrderByDescending(u => u.Address),
						("name", "desc") => query.OrderByDescending(u => u.Name),
						_ => query.OrderBy(u => u.Name),
					};

					totalUsers = await query.CountAsync();
					totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

					// Get paginated results
					if (pageSize == 100)
					{
						userList = await query.ToListAsync();
					}
					else
					{
						userList = await query
							.Skip((currentPage - 1) * pageSize)
							.Take(pageSize)
							.ToListAsync();
					}

					var userModels = new List<UserModel>();
					foreach (var user in userList)
					{
						var role = await authAcRepo.GetUserRole(user.IdentityId);
						userModels.Add(new UserModel
						{
							Id = user.Id,
							Name = user.Name,
							Email = user.Email,
							Role = role,
							PhoneNumber = user.PhoneNumber ?? "Not Provided",
							Address = user.Address ?? "No Address Provided",
						});
					}

					// Handle role-based sorting for non-role searches
					if (sortBy.Equals("role", StringComparison.OrdinalIgnoreCase))
					{
						userModels = (sortOrder.ToLower()) switch
						{
							"asc" => [.. userModels.OrderBy(u => u.Role)],
							"desc" => [.. userModels.OrderByDescending(u => u.Role)],
							_ => userModels
						};
					}

					var result = new PagedResult<UserModel>
					{
						Items = userModels,
						CurrentPage = currentPage,
						TotalPages = totalPages,
						PageSize = pageSize,
						TotalItems = totalUsers,
						StartItemIndex = totalUsers == 0 ? 0 : (currentPage - 1) * pageSize + 1,
						EndItemIndex = Math.Min(currentPage * pageSize, totalUsers),
						SortBy = sortBy,
						SortOrder = sortOrder,
						SearchTerm = searchTerm,
					};

					return PartialView("Partials/_UsersTable", result);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error in GetUsers: {ex}");
				return PartialView("Partials/_UsersTable", new PagedResult<UserModel>
				{
					Items = [],
					CurrentPage = 1,
					PageSize = pageSize,
					TotalItems = 0,
					TotalPages = 0,
					StartItemIndex = 0,
					EndItemIndex = 0,
					SearchTerm = searchTerm,
					SortBy = sortBy,
					SortOrder = sortOrder
				});
			}
		}



		// Temporary storage for users and rooms 
		// In a real application, this data would come from a database
        private static List<Room> _rooms = GetInitialRooms();


     

        public IActionResult Rooms(string searchTerm = "", string sortBy = "number", int page = 1)
        {
            var rooms = _rooms.AsQueryable();

            // Filter rooms
            if (!string.IsNullOrEmpty(searchTerm))
            {
                rooms = rooms.Where(r =>
                    r.Number.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    r.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Sort rooms
            rooms = sortBy.ToLower() switch
            {
                "type" => rooms.OrderBy(r => r.Type),
                "status" => rooms.OrderBy(r => r.Status),
                "price" => rooms.OrderBy(r => r.Price),
                _ => rooms.OrderBy(r => r.Number)
            };

            var roomsList = rooms.ToList();
            var itemsPerPage = 4;
            var totalRooms = roomsList.Count;
            var totalPages = (int)Math.Ceiling((double)totalRooms / itemsPerPage);
            var paginatedRooms = roomsList.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var viewModel = new RoomViewModel
            {
                Rooms = paginatedRooms,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRooms = totalRooms,
                ItemsPerPage = itemsPerPage
            };

            ViewBag.CurrentAction = "Rooms";
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] Room room)
        {
            if (ModelState.IsValid)
            {
                room.Id = _rooms.Count > 0 ? _rooms.Max(r => r.Id) + 1 : 1;

                // Parse amenities from string
                if (!string.IsNullOrEmpty(room.AmenitiesString))
                {
                    room.Amenities = room.AmenitiesString.Split(',')
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();
                }

                _rooms.Add(room);

                return Json(new { success = true, message = "Room created successfully!" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }


        private static List<Room> GetInitialRooms()
        {
            return new List<Room>
            {
                new Room { Id = 101, Number = "101", Type = "Standard", Status = "Available", Price = 120, Capacity = 2, Floor = 1, Amenities = new List<string> { "WiFi", "AC", "TV" } },
                new Room { Id = 102, Number = "102", Type = "Deluxe", Status = "Occupied", Price = 180, Capacity = 3, Floor = 1, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar" } },
                new Room { Id = 201, Number = "201", Type = "Suite", Status = "Available", Price = 300, Capacity = 4, Floor = 2, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar", "Balcony" } },
                new Room { Id = 202, Number = "202", Type = "Standard", Status = "Maintenance", Price = 120, Capacity = 2, Floor = 2, Amenities = new List<string> { "WiFi", "AC", "TV" } },
                new Room { Id = 301, Number = "301", Type = "Presidential", Status = "Available", Price = 500, Capacity = 6, Floor = 3, Amenities = new List<string> { "WiFi", "AC", "TV", "Mini Bar", "Balcony", "Jacuzzi" } }
            };
        }
    }

}